param(
    [int]$Affinity = 1024,
    [int]$IterationCount = 7,
    [int]$IterationTimeMs = 200,
    [int]$LaunchCount = 2,
    [string]$Filter = '*LodestarComparisonBenchmarks*',
    [switch]$VerifyOnly
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
$revision = '52d30eae62e4222014d789d30dd0de8c5e07bb4d'
$source = Join-Path $repo 'artifacts/lodestar-source'
$output = Join-Path $repo ('artifacts/lodestar-comparison/' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $output -Force | Out-Null
if (!(Test-Path -LiteralPath (Join-Path $source '.git'))) {
    git clone --filter=blob:none --no-checkout https://github.com/CyrilB1531/lodestar.git $source
    if ($LASTEXITCODE) { throw 'Lodestar clone failed' }
}
git -C $source sparse-checkout set src/Lodestar.Fuzzy src/Lodestar.Text src/Lodestar.Abstractions src/Shared
if ($LASTEXITCODE) { throw 'Sparse checkout failed' }
git -C $source fetch origin $revision --depth 1
if ($LASTEXITCODE) { throw 'Cannot fetch pinned Lodestar revision' }
git -C $source checkout --detach $revision
if ($LASTEXITCODE) { throw 'Lodestar checkout failed' }
dotnet build (Join-Path $source 'src/Lodestar.Fuzzy/Lodestar.Fuzzy.csproj') -c Release -f net10.0 -p:GeneratePackageOnBuild=false -v minimal
if ($LASTEXITCODE) { throw 'Lodestar build failed' }
$oldRoot = $env:FuzzySharpLodestarRoot
$oldBaseline = $env:FuzzySharpPerformanceBaseline
try {
    $env:FuzzySharpLodestarRoot = Join-Path $source 'src/Lodestar.Fuzzy/bin/Release/net10.0'
    $env:FuzzySharpPerformanceBaseline = $null
    $project = Join-Path $PSScriptRoot 'FuzzySharp.Benchmarks.csproj'
    dotnet build $project -c Release -t:Rebuild -p:GeneratePackageOnBuild=false -v minimal
    if ($LASTEXITCODE) { throw 'Benchmark build failed' }
    @("Lodestar revision: $revision", "Raffinert revision: $(git -C $repo rev-parse HEAD)",
      "Affinity: $Affinity; iterations: $IterationCount; target ms: $IterationTimeMs; launches: $LaunchCount",
      'JIT tiering/PGO disabled; no outlier removal; scores compared with tolerance 1e-9.',
      (git -C $repo diff --stat)) | Set-Content (Join-Path $output 'run.txt')
    dotnet run --project $project -c Release --no-build -- --verify-lodestar > (Join-Path $output 'scores.csv')
    if ($LASTEXITCODE) { throw 'Score verification failed' }
    $differences = @(Import-Csv (Join-Path $output 'scores.csv') | Where-Object Equal -NE 'True')
    if ($differences.Count) { throw "Score differences found; inspect $output/scores.csv before timing" }
    dotnet run --project $project -c Release --no-build -- --verify-lodestar-extraction > (Join-Path $output 'extraction-check.txt')
    if ($LASTEXITCODE) { throw 'Extraction agreement check failed' }
    dotnet run --project $project -c Release --no-build -- --probe-lodestar-ties > (Join-Path $output 'ties.txt')
    if ($LASTEXITCODE) { throw 'Tie probe failed' }
    if (!$VerifyOnly) {
        dotnet run --project $project -c Release --no-build -- --filter $Filter --affinity $Affinity --envVars DOTNET_TieredCompilation:0 --iterationCount $IterationCount --iterationTime $IterationTimeMs --warmupCount 5 --launchCount $LaunchCount --outliers DontRemove --artifacts (Join-Path $output 'benchmarks')
        if ($LASTEXITCODE) { throw 'Benchmark run failed' }
        $reports = @(Get-ChildItem (Join-Path $output 'benchmarks/results') -Filter '*report.csv')
        if (!$reports.Count) { throw 'No benchmark results' }
        foreach ($report in $reports) {
            if (@(Import-Csv $report.FullName | Where-Object Mean -NotMatch '\d').Count) { throw 'Missing benchmark measurements' }
        }
    }
}
finally {
    $env:FuzzySharpLodestarRoot = $oldRoot
    $env:FuzzySharpPerformanceBaseline = $oldBaseline
}
Write-Host "Results: $output"
