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

            rootFiles.AddRange(Directory.GetFiles(workingDir, "*.cs", SearchOption.TopDirectoryOnly));

            var directories = Directory.GetDirectories(workingDir, "*", SearchOption.AllDirectories)
                .Where(p => !p.Contains("obj", StringComparison.OrdinalIgnoreCase)
                && !p.Contains("bin", StringComparison.OrdinalIgnoreCase));

            foreach (var d in directories)
            {
                rootFiles.AddRange(Directory.GetFiles(d, "*.cs", SearchOption.TopDirectoryOnly));
            }

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

        private static void CleanIntermediateOutputPath(string intermediateOutputPath)
        {
            if (Directory.Exists(intermediateOutputPath))
            {
                Directory.Delete(intermediateOutputPath, true);
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

            //TODO: Filter not expired generated file
            //NOTE: This is actually a shortcut. We delete everything and generate file.
            //This way we are sure that if a file has been deleted on project side, there is no "ghost file".
            //We must check 2 files tree and add/remove/update correctly
            //As we may not have a lot of extra file to generate to native this is not a priority yet.
            CleanIntermediateOutputPath(finalOutputDir);

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
