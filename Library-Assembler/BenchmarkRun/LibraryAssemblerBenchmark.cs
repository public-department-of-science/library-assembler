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
        string stringValue;

        //    [Params(@"G:\DeskTop\grammar-analyzer\GrammarAnalyzer\04.Samples.CodePlace\BenchmarkReadingSamples\sample-text-file.txt",
        //@"G:\DeskTop\grammar-analyzer\GrammarAnalyzer\04.Samples.CodePlace\BenchmarkReadingSamples\sample-2mb-text-file.txt")]
        //    public string pathToTheFile;

        //[GlobalSetup]
        //public void ReadFileAsString()
        //{
        //    stringValue = File.ReadAllText(pathToTheFile);
        //}

        [Benchmark]
        public void StringBuilderFixedSize()
        {
            Program.Main();
        }
    }
}