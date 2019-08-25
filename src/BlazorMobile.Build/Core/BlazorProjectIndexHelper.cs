using BlazorMobile.Build.Core.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace BlazorMobile.Build.Server.Core
{
    internal static class BlazorProjectIndexHelper
    {
        public static void FindAndReplace(string inputDir)
        {
            if (!Directory.Exists(inputDir))
            {
                throw new InvalidOperationException("The input directory does not exist");
            }

            string indexFile = GetBlazorWebAssemblyIndexFilePath(inputDir);

            string outputFile = TransformAndCopy(indexFile, inputDir);

            Console.WriteLine($"BlazorMobile.Build -> {FileToReplace} to {NewFile} done in {Path.GetDirectoryName(outputFile)}");
        }

        private const string FileToReplace = "index.html";
        private const string NewFile = "server_" + FileToReplace;
        private const string SearchedOccurence = "blazor.webassembly.js";
        private const string NewOccurence = "blazor.server.js";

        public static string GetBlazorWebAssemblyIndexFilePath(string inputDir)
        {
            //Assuming the file we seach will only be index.html
            //Prioritizing found files with lowest recursion
            var results = Directory.GetFiles(inputDir, FileToReplace, SearchOption.TopDirectoryOnly);

            if (results.Length <= 0)
            {
                throw new FileNotFoundException($"BlazorMobile.Build -> No {FileToReplace} files were found");
            }

            string indexFile = null;

            foreach (var currentFile in results)
            {
                var content = File.ReadAllText(currentFile);
                if (content.Contains(SearchedOccurence))
                {
                    indexFile = currentFile;
                    break;
                }
            }

            if (string.IsNullOrEmpty(indexFile))
            {
                throw new FileNotFoundException($"BlazorMobile.Build -> No suitable {FileToReplace} files were found");
            }

            return indexFile;
        }

        public static string TransformAndCopy(string sourceFile, string outputDir)
        {
            string outputFile = outputDir + Path.DirectorySeparatorChar + FileToReplace;

            try
            {
                var encoding = TextFileEncodingDetector.DetectTextFileEncoding(sourceFile);
                string content = File.ReadAllText(sourceFile, encoding);
                File.WriteAllText(outputDir + Path.DirectorySeparatorChar + NewFile, $"<!-- AUTO-GENERATED FILE - DO NOT EDIT! -->{Environment.NewLine}" + content.Replace(SearchedOccurence, NewOccurence), encoding);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unable to update destination file {outputFile}", ex);
            }

            return outputFile;
        }
    }
}
