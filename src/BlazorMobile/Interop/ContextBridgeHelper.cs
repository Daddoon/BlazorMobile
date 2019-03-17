using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("BlazorMobile.Android")]
[assembly: InternalsVisibleTo("BlazorMobile.iOS")]
[assembly: InternalsVisibleTo("BlazorMobile.UWP")]
namespace BlazorMobile.Interop
{
    internal static class ContextBridgeHelper
    {
        private static string JsFilesPath = "Interop.Javascript.";

        private static string GetFileContent(string filename)
        {
            var assembly = typeof(ContextBridgeHelper).Assembly;

            //Assembly name and Assembly namespace differ in this project
            string JsNamespace = $"BlazorMobile.{JsFilesPath}";

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


        public static string GetInjectableJavascript(bool isAnonymousAutoEvalMethod = true)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GetFileContent(MainResourceFile));
            sb.AppendLine();

            //switch (Device.RuntimePlatform)
            //{
            //    case Device.UWP:
            //        sb.Append(GetFileContent(UWPResourceFile));
            //        break;
            //    case Device.Android:
            //        sb.Append(GetFileContent(AndroidResourceFile));
            //        break;
            //    case Device.iOS:
            //        sb.Append(GetFileContent(iOSResourceFile));
            //        break;
            //}

            sb.AppendLine();
            var content = sb.ToString();


            if (!isAnonymousAutoEvalMethod)
                return content;

            content = $"(function() {{ {content} }})();";

            //switch (Device.RuntimePlatform)
            //{
            //    case Device.Android:
            //        content = "var xInit = " + content;
            //        break;
            //    case Device.iOS:
            //        content = $"var xInit = setInterval(function () " +
            //            $"{{ " +
            //                $"if (Blazor == null || Blazor == undefined) {{ return; }}" +
            //                $"else {{ {content} clearInterval(xInit); }}" +
            //            $"}}, 10);";
            //        break;
            //    case Device.UWP:
            //    default:
            //        break;
            //}

            return content;
        }
    }
}
