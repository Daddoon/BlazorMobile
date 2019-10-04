using BlazorMobile.Components;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using Xamarin.Forms;
using BlazorMobile.Interop;
using System.Threading.Tasks;
using BlazorMobile.Services;
using BlazorMobile.Common.Interop;
using BlazorMobile.Controller;
using BlazorMobile.Common.Serialization;

[assembly: InternalsVisibleTo("BlazorMobile.Android")]
[assembly: InternalsVisibleTo("BlazorMobile.iOS")]
[assembly: InternalsVisibleTo("BlazorMobile.UWP")]
[assembly: InternalsVisibleTo("BlazorMobile.ElectronNET")]
namespace BlazorMobile.Helper
{
    //TODO: Remove this comment when shipping BlazorMobile 3.0.11.
    //Typically, this helper is missing in this current minor release but we want to add
    //some internals for PostMessage API. A conflict will occur when merging with the next release
    internal static class WebViewHelper
    {
        public static void PostMessage(string messageName, Type TArgsType, object[] args)
        {
            MessageForwarder(new MessageProxy(messageName, TArgsType, args));
        }

        public static void CallJSInvokableMethod(string assembly, string method, params object[] args)
        {
            MessageForwarder(new MessageProxy(assembly, method, args));
        }

        private static void MessageForwarder(MessageProxy messageProxy)
        {
            BlazorContextBridge.Current.SendMessageToClient(
              BridgeSerializer.Serialize(messageProxy));
        }
    }
}