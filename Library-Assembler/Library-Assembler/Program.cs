using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Assembler
{
    public class Program
    {
        // pri #3
        private static string[] skipByEquality = new string[] { "bin", "release", "debug", "obj", ".editorconfig", ".vs", "form", "Form" };
        // pri #2
        private static string[] skipByPathSegmentContains = new string[] { "TemporaryGeneratedFile", "NET", "AssemblyAttributes", "AssemblyInfo", "Class1",
            "git", "Benchmark", "benchmark", "Gui", "gui", "NETCoreApp", "Forms", "Test","forms",};
        // pri #1
        private static string[] skipExtensions = new string[] { "txt", "jpg", "jpeg", "png", "dgml" };

        /// <summary>
        /// ... 6-7MB - works wrong
        /// | 37.98 ms | 0.573 ms | 0.536 ms | 1285.7143 GC(0)|                  5 MB - works wrong
        /// | 33.79 ms | 0.088 ms | 0.078 ms | 200.0000 GC(0) | 66.6667 GC(1) |  929 KB - works wrong
        /// | 28.94 ms | 0.139 ms | 0.130 ms | 156.2500       | 31.2500       |  783 KB - works not so good
        /// | 241.1 ms | 8.74 ms  | 24.07 ms |  1 MB | - perfect copy
        /// | 243.0 ms | 6.49 ms | 17.78 ms  |  1 MB | - decrease error propagation ms
        /// | 242.0 ms | 5.71 ms | 15.63 ms  |  1 MB | - remove Linq and take step over to not create var in loop
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

        private static bool IsFileExcluded(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || skipExtensions.Contains(extension))
            {
                return true;
            }

            string filePath = Path.GetDirectoryName(fileName);
            foreach (var containsWord in skipByPathSegmentContains)
            {
                if (fileName.Contains(containsWord, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            var splitFilePath = filePath.Split("\\");
            var excludeSegmentFound = false;
            foreach (var segment in splitFilePath)
            {
                foreach (var skipWord in skipByEquality)
                {
                    excludeSegmentFound = skipWord.Equals(segment);
                    if (excludeSegmentFound)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static IEnumerable<string> GetFiles(string path,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            var files = Directory.EnumerateFiles(path, "*.cs", searchOption).Where(f => !IsFileExcluded(f));
            return files;
        }
    }
}
