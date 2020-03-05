using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("BlazorMobile.Android")]
[assembly: InternalsVisibleTo("BlazorMobile.iOS")]
[assembly: InternalsVisibleTo("BlazorMobile.Web")]
namespace BlazorMobile.Interop
{
    internal static class ContextBridgeHelper
    {
        internal const string _contextBridgeRelativeURI = "/contextBridge";

        private const string JsFilesPath = "Interop.Javascript.";

        private static string GetFileContent(string filename)
        {
            var assembly = typeof(ContextBridgeHelper).Assembly;

            //Assembly name and Assembly namespace differ in this project
            string JsNamespace = $"BlazorMobile.Common.{JsFilesPath}";

            using (var contentStream = assembly.GetManifestResourceStream($"{JsNamespace}{filename}"))
            {
                using (var streamReader = new StreamReader(contentStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        private const string MainResourceFile = "contextbridge.js";
        private const string BlazorXamarinResourceFile = "blazormobile.xamarin.js";


        public static string GetContextBridgeJavascript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GetFileContent(MainResourceFile));
            sb.AppendLine();

            sb.AppendLine();
            return sb.ToString();
        }

        public static string GetBlazorXamarinJavascript()
        {
            return GetFileContent(BlazorXamarinResourceFile);
        }
    }
}
