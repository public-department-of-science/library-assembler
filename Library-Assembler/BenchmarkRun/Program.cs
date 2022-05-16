using BenchmarkDotNet.Running;
using Common.Benchmarking;

public static class BenchmarkRun
{
    public static void Main()
    {
        //var summary = BenchmarkRunner.Run<FIleReadOperations>();
        var summary1 = BenchmarkRunner.Run<LibraryAssemblerBenchmark>();
        //var summary2 = BenchmarkRunner.Run<StringBulderBenchmark>();
    }
}