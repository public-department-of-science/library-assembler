using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Assembler
{
    public class Program
    {
        private static string[] notAllowedKeywords = new string[] { "bin", "release", "debug", "txt", "jpg", "jpeg", "png",
                "git", "obj", "dgml", ".editorconfig", ".vs", "Forms", "Test","forms", "Benchmark",
            "benchmark", "form", "Form", "Gui", "gui", "NETCoreApp", "NET", "AssemblyAttributes",
        "AssemblyInfo", "Class1", "TemporaryGeneratedFile"};

        /// <summary>
        /// | 37.98 ms | 0.573 ms | 0.536 ms | 1285.7143 GC(0)|                  5 MB
        /// | 33.79 ms | 0.088 ms | 0.078 ms | 200.0000 GC(0) | 66.6667 GC(1) |  929 KB
        /// | 28.94 ms | 0.139 ms | 0.130 ms | 156.2500       | 31.2500       |  783 KB
        /// </summary>
        public static void Main()
        {
            string solutionFolderPath = @"G:\DeskTop\grammar-analyzer\GrammarAnalyzer";

            var isFileExtensionFound = solutionFolderPath.Contains('.');
            if (isFileExtensionFound)
            {
                solutionFolderPath = string.Join(@"\", solutionFolderPath.Split(new string[] { ":", "\\", " / ", "//", @"\" },
                    StringSplitOptions.RemoveEmptyEntries)[..^1]);
            }

            var isSolutionFolderFound = Directory.EnumerateFiles(solutionFolderPath).FirstOrDefault(x => x.Contains(".sln")) != null;
            if (isSolutionFolderFound == false)
            {
                throw new Exception(@"The path isn't correct cause impossible to find .sln file here.");
            }

            var pathToCopy = string.Concat(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), "AssemblyFiles");

            var isDirectoryExists = Directory.Exists(pathToCopy);
            if (isDirectoryExists)
            {
                Directory.Delete(pathToCopy, recursive: true);
                Directory.CreateDirectory(pathToCopy);
            }
            else
            {
                Directory.CreateDirectory(pathToCopy);
            }

            foreach (var file in GetFiles(solutionFolderPath))
            {
                var fileName = string.Concat(pathToCopy, "\\", Path.GetFileName(file));
                File.Copy(file, fileName, overwrite: true);
            }
        }

        private static bool IsExcluded(string fileName)
        {
            foreach (var forbiddenWord in notAllowedKeywords)
            {
                if (fileName.Contains(forbiddenWord, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            string extension = Path.GetExtension(fileName);
            return string.IsNullOrWhiteSpace(extension);
        }

        public static IEnumerable<string> GetFiles(string path,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            var files = Directory.EnumerateFiles(path, "*.cs", searchOption).Where(f => !IsExcluded(f));
            return files;
        }
    }
}
