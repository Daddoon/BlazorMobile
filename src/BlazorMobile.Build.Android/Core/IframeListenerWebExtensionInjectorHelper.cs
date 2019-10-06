using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BlazorMobile.Build.Android.Core
{
    public static class IframeListenerWebExtensionInjectorHelper
    {
        private const string BlazorMobileAndroidWebExtensionFolder = "BlazorMobile\\web_extensions";

        public static string GetIntermediateOutputPath(string projectFile, string baseIntermediateOutputPath)
        {
            string trimedPath = baseIntermediateOutputPath.TrimStart('\\');

            if (Path.IsPathRooted(trimedPath))
            {
                //Absolute path case
                return baseIntermediateOutputPath + BlazorMobileAndroidWebExtensionFolder;
            }
            else
            {
                //Relative path case (the default one)
                return Path.GetDirectoryName(projectFile) + Path.DirectorySeparatorChar + baseIntermediateOutputPath + BlazorMobileAndroidWebExtensionFolder;
            }
        }

        private static void CleanIntermediateOutputPath(string intermediateOutputPath)
        {
            if (Directory.Exists(intermediateOutputPath))
            {
                Directory.Delete(intermediateOutputPath, true);
            }
        }

        private readonly static string WebExtensionBaseRelativeResourcesPath = "BlazorMobile" + Path.DirectorySeparatorChar + "web_extensions" + Path.DirectorySeparatorChar + "iframe_listener" + Path.DirectorySeparatorChar;

        private static void CopyRequiredResources(string nugetPackageDir, string intermediateOutputPath)
        {
            //Create base WebExtension folder
            if (!Directory.Exists(intermediateOutputPath))
            {
                Directory.CreateDirectory(intermediateOutputPath);
            }

            #region Iframe WebExtension copy

            string iframeListenerDeepestFolder = intermediateOutputPath + WebExtensionBaseRelativeResourcesPath + "icons";

            if (!Directory.Exists(iframeListenerDeepestFolder))
            {
                Directory.CreateDirectory(iframeListenerDeepestFolder);
            }

            //TODO: Need timestamp strategy instead of overwriting

            File.Copy(
                nugetPackageDir + WebExtensionBaseRelativeResourcesPath + "manifest.json",
                intermediateOutputPath + WebExtensionBaseRelativeResourcesPath + "manifest.json", true);
            
            File.Copy(
                nugetPackageDir + WebExtensionBaseRelativeResourcesPath + "background.js",
                intermediateOutputPath + WebExtensionBaseRelativeResourcesPath + "background.js", true);
            
            File.Copy(
                nugetPackageDir + WebExtensionBaseRelativeResourcesPath + "icons" + Path.DirectorySeparatorChar + "blazormobile-48.png",
                intermediateOutputPath + WebExtensionBaseRelativeResourcesPath + "icons" + Path.DirectorySeparatorChar + "blazormobile-48.png", true);

            #endregion
        }


        public static void CopyOrSkipIframeListenerExtension(string nugetPackageDir, string intermediateOutputPath, string projectFile)
        {
            if (string.IsNullOrEmpty(nugetPackageDir) || !Directory.Exists(nugetPackageDir))
            {
                throw new InvalidOperationException("The specified NuGet package directory is invalid or does not exist");
            }
            
            if (string.IsNullOrEmpty(projectFile) || !File.Exists(projectFile))
            {
                throw new InvalidOperationException("The specified project is invalid or does not exist");
            }

            var finalOutputDir = GetIntermediateOutputPath(projectFile, intermediateOutputPath);

            //TODO: Need to check if file are outdated before cleaning the path
            CleanIntermediateOutputPath(finalOutputDir);

            CopyRequiredResources(nugetPackageDir, intermediateOutputPath);
        }
    }
}
