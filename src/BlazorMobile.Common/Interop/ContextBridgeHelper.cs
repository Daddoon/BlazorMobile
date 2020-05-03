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
    }
}
