using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BlazorMobile.Build.Core
{
    internal static class PublishAndZipHelper
    {
        internal const string artifactName = "app";

        internal const string artifactZipName = artifactName + ".zip";

        internal const string artifactFolderName = artifactName + "_folder";

        public static void PublishAndZip(string inputFile, string outputPath, string configuration)
        {
            if (!File.Exists(inputFile) || !inputFile.EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException("The input file does not exist or is not a Blazor csproj file");
            }

            //Warning invalid outputPath
            if (string.IsNullOrEmpty(outputPath))
            {
                throw new InvalidOperationException("The outputPath is not set");
            }

            //If the base output directory does not exist, create it
            if (!Directory.Exists(outputPath))
            {
                try
                {
                    Directory.CreateDirectory(outputPath);
                }
                catch (Exception)
                {
                    //Let's bubble up
                    throw;
                }
            }

            //If the base ouput exist, assuming we need to clean everything in it if we do consecutive call
            ClearDirectory(outputPath);

            //If the base temp publish output directory does not exist, create it. As we cleaned previous build, this should create it
            string artifactPublishTempDirectory = GetArtifactPublishTempFolderAbsolutePath(outputPath);
            if (!Directory.Exists(artifactPublishTempDirectory))
            {
                try
                {
                    Directory.CreateDirectory(artifactPublishTempDirectory);
                }
                catch (Exception)
                {
                    //Let's bubble up
                    throw;
                }
            }

            //Publish folder
            if (!PublishInTempFolder(inputFile, artifactPublishTempDirectory, configuration))
            {
                Console.WriteLine("ERROR: The BlazorMobile.Build temp publish task did not ended with success");
                return;
            }

            //TODO: Zip only the good folder and create the zip in the right directory

            //TODO: Clear only app_folder temp path after build
        }

        internal static bool PublishInTempFolder(string projectPath, string outputPath, string configuration)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {				 
                    FileName = "dotnet",
                    Arguments = $"publish \"{projectPath}\" -o \"{outputPath}\" -c {configuration}",
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            process.WaitForExit();

            int result = process.ExitCode;
            if (result != 0)
            {
                return false;
            }

            return true;
        }

        private static string GetArtifactZipAbsolutePath(string directory)
        {
            directory = directory.TrimEnd(Path.DirectorySeparatorChar);

            return directory + Path.DirectorySeparatorChar + artifactZipName;
        }

        private static string GetArtifactPublishTempFolderAbsolutePath(string directory)
        {
            directory = directory.TrimEnd(Path.DirectorySeparatorChar);

            return directory + Path.DirectorySeparatorChar + artifactFolderName;
        }

        private static void ClearDirectory(string directoryToClear)
        {
            try
            {
                //Remove artifact result
                var absolutePathToArtifactZipFile = GetArtifactZipAbsolutePath(directoryToClear);
                if (File.Exists(absolutePathToArtifactZipFile))
                {
                    File.Delete(absolutePathToArtifactZipFile);
                }

                //Remove publish result
                var absolutePathToArtifactPublishFolder = GetArtifactPublishTempFolderAbsolutePath(directoryToClear);
                if (Directory.Exists(absolutePathToArtifactPublishFolder))
                {
                    Directory.Delete(absolutePathToArtifactPublishFolder, true);
                }
            }
            catch (Exception)
            {
                //Bubble up
                throw;
            }
        }
    }
}
