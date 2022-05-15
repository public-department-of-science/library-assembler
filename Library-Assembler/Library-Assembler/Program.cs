using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Assembler
{
    class Program
    {
        static void Main()
        {
            string solutionFolderPath = @"G:\DeskTop\grammar-analyzer\GrammarAnalyzer";

            var isFileExtensionFound = solutionFolderPath.Contains('.');
            if (isFileExtensionFound)
            {
                solutionFolderPath = string.Join(@"\", solutionFolderPath.Split(new string[] { ":", "\\", " / ", "//", @"\" },
                    StringSplitOptions.RemoveEmptyEntries)[..^1]);
            }

            // check if .sln file was found
            var solutionFiles = Directory.GetFiles(solutionFolderPath);
            var isSolutionFolderFound = solutionFiles.FirstOrDefault(x => x.Contains(".sln")) != null;
            if (isSolutionFolderFound == false)
            {
                throw new Exception(@"The path isn't correct cause impossible to find .sln file here.");
            }

            var projectsToCopy = Directory.GetDirectories(solutionFolderPath);

            //  string[] files = System.IO.Directory.GetFiles(currentDirName, "*.txt");

            //foreach (string file in Directory.EnumerateFiles(solutionFolderPath, @"(?!\bword\b).cs", new EnumerationOptions()
            //{
            //    RecurseSubdirectories = true,
            //    ReturnSpecialDirectories = false,

            //}))
            //{
            //    Console.WriteLine(file);
            //}
            foreach (var item in GetFiles(solutionFolderPath, extenstionInclude: ".cs", exclude: new string[] { "bin" }))
            {
                Console.WriteLine(item);
            }

            var pathToCopy = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
        }

        private static bool IsExcluded(string fileName, string extenstionInclude, string[] exclude)
        {
            string[] notAllowedKeywords = new string[] { "bin", "release", "debug", "txt", "jpg", "jpeg", "png",
                "git", "obj", "dgml", ".editorconfig", ".vs"};
            if (exclude.Contains(Path.GetFileName(fileName)))
            {
                return true;
            }

            string extension = Path.GetExtension(fileName);
            if (extension.Equals(extenstionInclude) == false)
            {
                return true;
            }

            foreach (var forbiddenWord in notAllowedKeywords)
            {
                if (fileName.Contains(forbiddenWord, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return string.IsNullOrWhiteSpace(extension) || exclude.Contains("*" + extension);
        }

        public static IEnumerable<string> GetFiles(string path, string extenstionInclude, string[] exclude = null,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            var files = Directory.EnumerateFiles(path, "*.*", searchOption);
            if (exclude != null && exclude.Length > 0)
            {
                files = files.Where(f => !IsExcluded(f, extenstionInclude, exclude));
            }
            return files;
        }
    }
}
