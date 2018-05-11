using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("Daddoon.Blazor.Xamarin.Android")]
[assembly: InternalsVisibleTo("Daddoon.Blazor.Xamarin.iOS")]
[assembly: InternalsVisibleTo("Daddoon.Blazor.Xamarin.UWP")]
namespace Daddoon.Blazor.Xam.Interop
{
    internal static class ContextBridgeHelper
    {
        private static string JsFilesPath = "Interop.Javascript.";

        private static string GetFileContent(string filename)
        {
            var assembly = typeof(ContextBridgeHelper).Assembly;

            //Assembly name and Assembly namespace differ in this project
            string JsNamespace = $"Daddoon.Blazor.Xam.{JsFilesPath}";

            using (var contentStream = assembly.GetManifestResourceStream($"{JsNamespace}{filename}"))
            {
                using (var streamReader = new StreamReader(contentStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        private const string MainResourceFile = "contextbridge.js";
        private const string AndroidResourceFile = "contextbridge.android.js";
        private const string iOSResourceFile = "contextbridge.ios.js";
        private const string UWPResourceFile = "contextbridge.uwp.js";


        public static string GetInjectableJavascript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GetFileContent(MainResourceFile));
            sb.AppendLine();
            sb.AppendLine();

            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    sb.Append(GetFileContent(UWPResourceFile));
                    break;
                case Device.Android:
                    sb.Append(GetFileContent(AndroidResourceFile));
                    break;
                case Device.iOS:
                    sb.Append(GetFileContent(iOSResourceFile));
                    break;
            }

            sb.AppendLine();
            return sb.ToString();
        }
    }
}
