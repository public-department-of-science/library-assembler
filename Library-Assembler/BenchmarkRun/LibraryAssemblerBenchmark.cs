using BenchmarkDotNet.Attributes;
using Library.Assembler;
using System.IO;
using System.Text;

namespace Common.Benchmarking
{
    [MemoryDiagnoser]
    [KeepBenchmarkFiles]
    public class LibraryAssemblerBenchmark
    {
        [Benchmark]
        public void StringBuilderFixedSize()
        {
            Program.Main();
        }
    }
}