using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BlazorMobile.Build.ElectronNET.Core
{
    public static class SyncMissingBlazorMobileAssembliesHelper
    {
        public static string GetIntermediateOutputPath(string projectFile, string baseIntermediateOutputPath)
        {
            string trimedPath = baseIntermediateOutputPath.TrimStart('\\');

            if (Path.IsPathRooted(trimedPath))
            {
                //Absolute path case
                return baseIntermediateOutputPath;
            }
            else
            {
                //Relative path case (the default one)
                return Path.GetDirectoryName(projectFile) + Path.DirectorySeparatorChar + baseIntermediateOutputPath;
            }
        }

        public static string GetHostOutputDirFromIntermediateOutputDir(string absoluteIntermediateOutputDir)
        {
            //Removing possible trail
            absoluteIntermediateOutputDir = absoluteIntermediateOutputDir.TrimEnd('\\').TrimEnd('/');

            //We should remove the targeted framework, then the configuration type level

            //Removing platform
            absoluteIntermediateOutputDir = Path.GetDirectoryName(absoluteIntermediateOutputDir);

            //Removing configuration
            absoluteIntermediateOutputDir = Path.GetDirectoryName(absoluteIntermediateOutputDir);

            //Adding the Host output path
            absoluteIntermediateOutputDir += Path.DirectorySeparatorChar + "Host" + Path.DirectorySeparatorChar + "bin";

            return absoluteIntermediateOutputDir;
        }

        private static void CopyRequiredResources(string binDir, string intermediateOutputPath)
        {
            if (!Directory.Exists(intermediateOutputPath))
            {
                Directory.CreateDirectory(intermediateOutputPath);
            }

            var allowedExtensions = new[] { ".xml", ".pdb", ".dll" };
            var files = Directory
                .GetFiles(binDir)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                .ToList();

            foreach (string file in files)
            {
                string outputCurrentFile = Path.Combine(intermediateOutputPath, file.Replace(binDir, string.Empty));

                if (File.Exists(file))
                {
                    try
                    {
                        File.Copy(file, outputCurrentFile, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }


        public static void CopyMissingAssemblies(string projectFile, string intermediateOutputPath, string binDir)
        {           
            if (string.IsNullOrEmpty(projectFile) || !File.Exists(projectFile))
            {
                return;
            }

            var computedOutputDir = GetIntermediateOutputPath(projectFile, intermediateOutputPath);
            var hostOutputDir = GetHostOutputDirFromIntermediateOutputDir(computedOutputDir);

            CopyRequiredResources(binDir, hostOutputDir);
        }
    }
}
