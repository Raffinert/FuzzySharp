using BenchmarkDotNet.Running;
using Raffinert.FuzzySharp.Benchmarks.Docker.LevenshteinDistance;

namespace Raffinert.FuzzySharp.Benchmarks.Docker;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== FuzzySharp Docker Benchmark Runner ===");
        Console.WriteLine($"Started at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        Console.WriteLine($"Runtime: {Environment.Version}");
        Console.WriteLine($"OS: {Environment.OSVersion}");
        Console.WriteLine($"Machine: {Environment.MachineName}");
        Console.WriteLine($"User: {Environment.UserName}");
        Console.WriteLine($"Processor Count: {Environment.ProcessorCount}");
        Console.WriteLine($"Is 64-bit Process: {Environment.Is64BitProcess}");
        Console.WriteLine($"Is 64-bit OS: {Environment.Is64BitOperatingSystem}");
        Console.WriteLine();

        try
        {
            if (args.Length > 0 && args[0].ToLowerInvariant() == "list")
            {
                Console.WriteLine("Available benchmarks:");
                Console.WriteLine("- LevenshteinLargeDocker (default)");
                return;
            }

            Console.WriteLine("Running LevenshteinLargeDocker benchmark...");
            Console.WriteLine("This may take several minutes to complete.");
            Console.WriteLine();

            var summary = BenchmarkRunner.Run<LevenshteinLargeDocker>();

            Console.WriteLine();
            Console.WriteLine("=== Benchmark Completed ===");
            Console.WriteLine($"Completed at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

            if (summary.HasCriticalValidationErrors)
            {
                Console.WriteLine("❌ Benchmark completed with critical validation errors!");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("✅ Benchmark completed successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error running benchmark: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Environment.Exit(1);
        }
    }
}