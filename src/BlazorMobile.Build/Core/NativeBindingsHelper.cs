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

        private static List<string> GetReferencedProjects(string projectFile)
        {
            List<string> referencedProjects = new List<string>();

            try
            {
                XDocument projDefinition = XDocument.Load(projectFile);
                referencedProjects = projDefinition
                    .Element("Project")
                    .Elements("ItemGroup")
                    .Elements("ProjectReference")
                    .Attributes("Include")
                    .Select(p => p.Value)
                    .ToList();
            }
            catch (Exception)
            {
                //Ignore if the Project format is incorrect, we must target .NET Core SDK
            }

            return referencedProjects.Select(p => Path.GetDirectoryName(projectFile) + Path.DirectorySeparatorChar + p).ToList();
        }

        private static string GetProjectTargetFramework(string projectFile)
        {
            try
            {
                XDocument projDefinition = XDocument.Load(projectFile);
                string targetFramework = projDefinition
                    .Element("Project")
                    .Elements("PropertyGroup")
                    .Elements("TargetFramework")
                    .Select(p => p.Value)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(targetFramework))
                {
                    throw new InvalidOperationException("Unable to resolve the current TargetFramework");
                }

                return targetFramework;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("An error occured. The input project file is maybe not in .NET Core SDK format");
            }
        }

        private static string GetIntermediateOutputFolder(string projectFile, string currentConfiguration)
        {
            return "obj" + Path.DirectorySeparatorChar + currentConfiguration + Path.DirectorySeparatorChar + GetProjectTargetFramework(projectFile);
        }

        public static void GenerateNativeBindings(string projectFile, string currentConfiguration)
        {
            if (string.IsNullOrEmpty(projectFile) || !File.Exists(projectFile))
            {
                throw new InvalidOperationException("The specified project is invalid or does not exist");
            }

            string intermediateOutputFolder = GetIntermediateOutputFolder(projectFile, currentConfiguration);

            var finalOutputDir = Path.GetDirectoryName(projectFile) + Path.DirectorySeparatorChar + intermediateOutputFolder + Path.DirectorySeparatorChar + "BlazorMobileProxyClass";
            var referencedProjects = GetReferencedProjects(projectFile);

            //Add root Project
            referencedProjects.Insert(0, projectFile);

            foreach (var currentProject in referencedProjects)
            {
                string projectName = Path.GetFileNameWithoutExtension(currentProject);

                string workingDirectory = Path.GetDirectoryName(currentProject);

                //TODO: Filter not expired generation

                foreach (var file in GetAllCSharpFiles(workingDirectory))
                {
                    string relativeOutputPath = Path.GetDirectoryName(file.Replace(workingDirectory, string.Empty).TrimStart(Path.DirectorySeparatorChar));

                    //TEST FOLDER
                    BindingClassGenerator.GenerateBindingClass(file, finalOutputDir + Path.DirectorySeparatorChar + projectName + Path.DirectorySeparatorChar + relativeOutputPath);
                }
            }
        }
    }
}
