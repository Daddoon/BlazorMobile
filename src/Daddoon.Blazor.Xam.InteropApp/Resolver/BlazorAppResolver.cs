using System.IO;

namespace Daddoon.Blazor.Xam.Services
{
    public static class BlazorAppResolver
    {
        private static string BlazorPackageFolder = "Mobile.package.app.zip";

        public static Stream GetAppStream()
        {
            var assembly = typeof(BlazorAppResolver).Assembly;

            string appPackage = $"{assembly.GetName().Name}.{BlazorPackageFolder}";

            return assembly.GetManifestResourceStream(appPackage);
        }
    }
}
