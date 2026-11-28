param(
    [string] $BaselineRef = 'origin/master',
    [string] $Filter = '*RegressionBenchmarks*',
    [int] $IterationCount = 5,
    [int] $IterationTimeMs = 150
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$baselineCommit = & git -C $repoRoot rev-parse --verify "$BaselineRef^{commit}"
if ($LASTEXITCODE -ne 0) { throw "Cannot resolve baseline $BaselineRef" }
$runName = '{0}-{1}-{2}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), $baselineCommit.Substring(0, 12), [guid]::NewGuid().ToString('N').Substring(0, 8)
$runRoot = Join-Path $repoRoot "artifacts/performance/$runName"
$baselineRoot = Join-Path $runRoot 'baseline'
New-Item -ItemType Directory -Path $baselineRoot -Force | Out-Null
$archivePath = Join-Path $runRoot 'baseline.tar'
& git -C $repoRoot archive $baselineCommit FuzzySharp build Directory.Build.props README.md "--output=$archivePath"
if ($LASTEXITCODE -ne 0) { throw 'git archive failed' }
& tar -xf $archivePath -C $baselineRoot
if ($LASTEXITCODE -ne 0) { throw 'Baseline extraction failed' }
& dotnet build (Join-Path $baselineRoot 'FuzzySharp/FuzzySharp.csproj') -c Release -f NET10.0 -p:AssemblyName=FuzzySharp.PerformanceBaseline -p:GeneratePackageOnBuild=false -v minimal
if ($LASTEXITCODE -ne 0) { throw 'Baseline build failed' }

@(
    "Baseline ref: $BaselineRef",
    "Baseline commit: $baselineCommit",
    "Candidate commit: $(& git -C $repoRoot rev-parse HEAD)",
    'Candidate diff (includes local edits):',
    (& git -C $repoRoot diff --stat),
    "Filter: $Filter",
    "Iterations: $IterationCount; iteration time: $IterationTimeMs ms"
) | Set-Content -LiteralPath (Join-Path $runRoot 'run.txt') -Encoding utf8
$patchPath = Join-Path $runRoot 'candidate.patch'
& git -C $repoRoot diff "--output=$patchPath"
if ($LASTEXITCODE -ne 0) { throw 'Cannot record candidate diff' }

# Environment properties are inherited by BenchmarkDotNet's generated build.
$previousBaseline = $env:FuzzySharpPerformanceBaseline
try {
    $env:FuzzySharpPerformanceBaseline = Join-Path $baselineRoot 'FuzzySharp/bin/Release/net10.0/FuzzySharp.PerformanceBaseline.dll'
    Push-Location $repoRoot
    try {
        # Rebuild also refreshes deps.json when switching the optional baseline.
        & dotnet build (Join-Path $PSScriptRoot 'FuzzySharp.Benchmarks.csproj') -c Release -t:Rebuild -p:GeneratePackageOnBuild=false -v minimal
        if ($LASTEXITCODE -ne 0) { throw 'Candidate build failed' }
        & dotnet run --project (Join-Path $PSScriptRoot 'FuzzySharp.Benchmarks.csproj') -c Release --no-build -- --filter $Filter --iterationCount $IterationCount --iterationTime $IterationTimeMs --warmupCount 3 --launchCount 1 --artifacts (Join-Path $runRoot 'results')
        if ($LASTEXITCODE -ne 0) { throw 'Benchmark process failed' }
    }
    finally { Pop-Location }

    $resultFiles = @(Get-ChildItem (Join-Path $runRoot 'results/results') -Filter '*-report.csv')
    if ($resultFiles.Count -eq 0) { throw 'No benchmark results produced' }
    foreach ($resultFile in $resultFiles) {
        $rows = @(Import-Csv -LiteralPath $resultFile.FullName)
        if ($rows.Count -eq 0 -or @($rows | Where-Object { $_.Mean -notmatch '\d' }).Count -ne 0) {
            throw "Missing measurements in $($resultFile.FullName)"
        }
        if (@($rows | Where-Object Method -eq 'Before').Count -eq 0) {
            throw "Baseline missing from $($resultFile.FullName)"
        }
    }
}
finally { $env:FuzzySharpPerformanceBaseline = $previousBaseline }
Write-Host "Performance results: $runRoot"
