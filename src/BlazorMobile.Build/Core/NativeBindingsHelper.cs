using BlazorMobile.Build.Cli.Helper;
using BlazorMobile.Build.Core.NativeBindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BlazorMobile.Build.Core
{
    public static class NativeBindingsHelper
    {
        private static List<string> FilterEligibleFiles(List<string> projectFiles)
        {
            return projectFiles.Where(p => BindingClassGenerator.HasProxyInterfaces(p)).ToList();
        }

        private static IEnumerable<string> GetAllCSharpFiles(string workingDir)
        {
            List<string> rootFiles = new List<string>();

            //In order to avoid some locking issue while fetching existing directories through obj or bin
            //with the Directory.GetDirectories API, we fetch some directories data separately

            //If we were respecting some project integrity, we would try to only exclude the real
            //intermediate output path and real output folder. Instead we just ignore all directories
            //content  where the folder is called "obj" or "bin".

            //TODO: Scan csproj files instead and resolve from project files the correct variable values

            rootFiles.AddRange(DirectoryHelper.GetFiles(workingDir,
                "*.cs",
                new List<string>()
                {
                    "obj",
                    "bin"
                }));

            return FilterEligibleFiles(rootFiles);
        }

        private static void _GetReferencedProjects(string projectFile, List<string> projectList)
        {
            try
            {
                projectFile = Path.GetFullPath(projectFile);

                if (projectList.Contains(projectFile))
                    return;

                projectList.Add(projectFile);

                XDocument projDefinition = XDocument.Load(projectFile);
                var referencedProjects = projDefinition
                    .Element("Project")
                    .Elements("ItemGroup")
                    .Elements("ProjectReference")
                    .Attributes("Include")
                    .Select(p => Path.GetFullPath(Path.GetDirectoryName(projectFile) + Path.DirectorySeparatorChar + p.Value)) //Force absolute
                    .ToList();

                foreach (string project in referencedProjects)
                {
                    //Recursive search. As each sub-project will not be added twice if absolute path is seen twice it will end automatically
                    _GetReferencedProjects(project, projectList);
                }
            }
            catch (Exception)
            {
                //Ignore if the Project format is incorrect, we must target .NET Core SDK
            }
        }

        private static List<string> GetReferencedProjects(string projectFile)
        {
            List<string> referencedProjects = new List<string>();

            _GetReferencedProjects(projectFile, referencedProjects);

            return referencedProjects;
        }

        private const string BlazorMobileProxyClassFolderName = "BlazorMobileProxyClass";

        public static string GetIntermediateOutputPath(string projectFile, string baseIntermediateOutputPath)
        {
            string trimedPath = baseIntermediateOutputPath.TrimStart('\\');

            if (Path.IsPathRooted(trimedPath))
            {
                //Absolute path case
                return Path.Combine(baseIntermediateOutputPath, BlazorMobileProxyClassFolderName);
            }
            else
            {
                //Relative path case (the default one)
                return Path.Combine(Path.GetDirectoryName(projectFile), baseIntermediateOutputPath, BlazorMobileProxyClassFolderName);
            }
        }

        public static void GenerateNativeBindings(string projectFile, string intermediateOutputPath)
        {
            intermediateOutputPath = PathHelper.MSBuildQuoteFixer(intermediateOutputPath);

            if (string.IsNullOrEmpty(projectFile) || !File.Exists(projectFile))
            {
                throw new InvalidOperationException("The specified project is invalid or does not exist");
            }

            var finalOutputDir = GetIntermediateOutputPath(projectFile, intermediateOutputPath);

            foreach (var currentProject in GetReferencedProjects(projectFile))
            {
                string projectName = Path.GetFileNameWithoutExtension(currentProject);

                string workingDirectory = Path.GetDirectoryName(currentProject);

                foreach (var file in GetAllCSharpFiles(workingDirectory))
                {
                    string relativeOutputPath = Path.GetDirectoryName(file.Replace(workingDirectory, string.Empty).TrimStart(Path.DirectorySeparatorChar));

                    BindingClassGenerator.GenerateBindingClass(file, finalOutputDir + Path.DirectorySeparatorChar + projectName + Path.DirectorySeparatorChar + relativeOutputPath);
                }
            }
        }
    }
}
