using BlazorMobile.Build.Core.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazorMobile.Build.Server.Core
{
    internal static class BlazorProjectIndexHelper
    {
        public static void FindAndReplace(string inputDir, string projectFile)
        {
            string wwwFolder = inputDir + "/wwwroot";

            if (!Directory.Exists(wwwFolder))
            {
                throw new InvalidOperationException("The input directory does not exist");
            }

            string indexFile = GetBlazorWebAssemblyIndexFilePath(wwwFolder);

            string outputFile = TransformAndCopy(indexFile, inputDir, projectFile);

            Console.WriteLine($"BlazorMobile.Build -> {FileToReplace} to {NewFile} done in {Path.GetDirectoryName(outputFile)}");
        }

        private const string FileToReplace = "index.html";
        private const string NewFile = "server_index.cshtml";
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

        private static string AddCSHTMLCode(string content, string projectFile)
        {
            string crtNamespace = Path.GetFileNameWithoutExtension(projectFile);

            content = content.Replace(SearchedOccurence, NewOccurence);

            content =
                "@page \"/\"" + Environment.NewLine
                + $"@namespace {crtNamespace}" + Environment.NewLine
                + "@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers" + Environment.NewLine + Environment.NewLine
                + content;

            content = Regex.Replace(content, @"(?:<app>)(.*?)(?:</app>)", @"<app>@(await Html.RenderComponentAsync<MobileApp>(RenderMode.ServerPrerendered))</app>", RegexOptions.Singleline);

            content += $"{Environment.NewLine}<!-- AUTO-GENERATED FILE - DO NOT EDIT! -->";

            return content;
        }

        public static string TransformAndCopy(string sourceFile, string outputDir, string projectFile)
        {
            string outputFile = outputDir + Path.DirectorySeparatorChar + NewFile;

            try
            {
                var encoding = TextFileEncodingDetector.DetectTextFileEncoding(sourceFile);
                File.WriteAllText(
                    outputFile, 
                    AddCSHTMLCode(File.ReadAllText(sourceFile, encoding), projectFile)
                    , encoding);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unable to update destination file {outputFile}", ex);
            }

            return outputFile;
        }
    }
}
