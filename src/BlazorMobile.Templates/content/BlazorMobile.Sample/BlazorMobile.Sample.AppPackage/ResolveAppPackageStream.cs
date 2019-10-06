using System.IO;

namespace BlazorMobile.Sample.AppPackage
{
    public static class AppPackageHelper
    {
        public const string BlazorAppPackageName = "BlazorMobile.Sample.Blazor.zip";

        public static Stream ResolveAppPackageStream()
        {
            //This app assembly
            var assembly = typeof(AppPackageHelper).Assembly;

            //Name of our current Blazor package in this project, stored as an "Embedded Resource"
            //The file is resolved through AssemblyName.FolderAsNamespace.YourPackageNameFile
            //In this example, the result would be BlazorMobile.Sample.Package.BlazorMobile.Sample.Blazor.zip
            return assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Package.{BlazorAppPackageName}");
        }
    }
}
