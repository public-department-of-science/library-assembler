using BenchmarkDotNet.Attributes;
using Library.Assembler;

namespace Common.Benchmarking
{
    [MemoryDiagnoser]
    [KeepBenchmarkFiles]
    public class LibraryAssemblerBenchmark
    {
        [Benchmark]
        public void StringBuilderFixedSize()
        {
            Program.Main(new string[] { "AssembledFiles", @"G:\DeskTop\grammar-analyzer\GrammarAnalyzer", 
                @"G:\DeskTop\grammar-analyzer\GrammarAnalyzer\Grammar.Analyzer.Library" });
        }
    }
}