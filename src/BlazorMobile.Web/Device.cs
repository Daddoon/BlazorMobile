using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.JSInterop;
using Mono.WebAssembly.Interop;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorMobile.Common
{

    //Mimic Xamarin.Forms Device static class
    public static class Device
    {
        public const string iOS = "iOS";
        public const string Android = "Android";
        public const string WinPhone = "WinPhone";
        public const string UWP = "UWP";
        public const string Browser = "Browser";
        public const string Unknown = "Unknown";
        public static string RuntimePlatform { get; internal set; } = Unknown;

        internal static Action<bool> _onFinishCallback = null;

        internal static void Init(Action<bool> onFinish)
        {
            _onFinishCallback = onFinish;
        }
    }
}
