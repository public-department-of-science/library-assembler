using BenchmarkDotNet.Running;
using Common.Benchmarking;

public static class BenchmarkRun
{
    public static void Main()
    {
        var summary = BenchmarkRunner.Run<LibraryAssemblerBenchmark>();
    }
}