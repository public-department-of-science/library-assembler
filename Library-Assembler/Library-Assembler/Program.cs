using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            "git", "Benchmark", "benchmark", "Gui", "gui", "NETCoreApp", "Forms", "Test","forms", "Library"};
        // pri #1
        private static string[] skipExtensions = new string[] { "txt", "jpg", "jpeg", "png", "dgml", "csproj" };

        // ... 6-7MB - works wrong
        // | 37.98 ms | 0.573 ms | 0.536 ms | 1285.7143 GC(0)|                  5 MB
        // | 33.79 ms | 0.088 ms | 0.078 ms | 200.0000 GC(0) | 66.6667 GC(1) |  929 KB
        // | 28.94 ms | 0.139 ms | 0.130 ms | 156.2500       | 31.2500       |  783 KB
        // | 241.1 ms | 8.74 ms  | 24.07 ms |  1 MB | 
        // | 243.0 ms | 6.49 ms  | 17.78 ms |  1 MB | 
        // | 242.0 ms | 5.71 ms  | 15.63 ms |  1 MB |
        // 284.1 ms (General case) | 15.33 ms   | 43.50 ms | 267.8 ms (Median) | 1 MB |
        /// <summary>
        /// [0] - new project folder name; [1] - current assembly version; [2] - solution folder path; [3] - destination copy project point-path. </param>
        /// </summary>
        /// <param name="args">
        public static void Main(string[] args)
        {
            throw new Exception("Check values for the solution to copy from path and path for the destination point before the run.");

            if (args.Length != 4)
            {
                throw new Exception("Input arguments [Dest. folder name], [current version], [from where to copy]," +
                    " [where to paste] - weren't corrent formed, check it up.");
            }

            var fromWhereToCopy = ParseProgramSolutionPath(args[2]);
            var whereToPaste = ParsePasteProjectPath(args[0], args[1], args[3]);

            foreach (var file in GetFiles(fromWhereToCopy))
            {
                var fileName = string.Concat(whereToPaste, "\\", Path.GetFileName(file));
                File.Copy(file, fileName, overwrite: true);
            }
        }

        private static string ParseProgramSolutionPath(string pathSegment)
        {
            string solutionFolderPath = pathSegment;
            var isFileExtensionFound = solutionFolderPath.Contains('.');
            if (isFileExtensionFound)
            {
                solutionFolderPath = string.Join(@"\", solutionFolderPath.Split(new string[] { ":", "\\", " / ", "//", @"\" },
                    StringSplitOptions.RemoveEmptyEntries)[..^1]);
            }

            var isPathExists = Directory.Exists(solutionFolderPath);
            if (isPathExists == false)
            {
                throw new Exception($"Solution folder: {pathSegment} was wrong defined and do not exists yet.");
            }

            var isSolutionFolderFound = Directory.EnumerateFiles(solutionFolderPath).FirstOrDefault(x => x.Contains(".sln")) != null;
            if (isSolutionFolderFound == false)
            {
                throw new Exception(@"The path isn't correct cause impossible to find .sln file here.");
            }

            return solutionFolderPath;
        }

        private static string ParsePasteProjectPath(string copyFolderName, string assemblyVersion, string pastePathSegment)
        {
            string copyProjectFolderPath = string.Empty;
            foreach (var extension in skipExtensions)
            {
                var skipExtensionFound = pastePathSegment.Contains(extension, StringComparison.OrdinalIgnoreCase);
                if (skipExtensionFound)
                {
                    copyProjectFolderPath = string.Join(@"\", pastePathSegment.Split(new string[] { ":", "\\", "/", "//", @"\" },
                        StringSplitOptions.RemoveEmptyEntries)[..^2]); // cause we need to skip ext. and fileName
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(copyProjectFolderPath))
            {
                copyProjectFolderPath = string.Concat(Path.GetFullPath(Path.Combine(pastePathSegment, copyFolderName, assemblyVersion)));
            }
            else
            {
                copyProjectFolderPath = string.Concat(Path.GetFullPath(Path.Combine(copyProjectFolderPath, copyFolderName, assemblyVersion)));
            }

            if (copyProjectFolderPath.Contains("Library", StringComparison.OrdinalIgnoreCase) == false)
            {
                throw new Exception("Destination project path doesn't include 'Library' segment that mean it" +
                    " will be copied potentially in wrong destination point");
            }

            var isDirectoryExists = Directory.Exists(copyProjectFolderPath);
            if (isDirectoryExists)
            {
                Directory.Delete(copyProjectFolderPath, recursive: true);
                Directory.CreateDirectory(copyProjectFolderPath);
            }
            else
            {
                Directory.CreateDirectory(copyProjectFolderPath);
            }

            return copyProjectFolderPath;
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
