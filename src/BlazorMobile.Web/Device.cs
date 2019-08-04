using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.JSInterop;
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

        private static void InitCommon(Action<bool> onFinish)
        {
            //Other code has finally migrated in BlazorMobileComponent for Server only behavior compatibility in plugin initialization
            //_onFinishCallback will be taken from the static field. Not attended to be used in a pure multi-client web scenario of course
            _onFinishCallback = onFinish;
        }

        internal static void Init(IComponentsApplicationBuilder app, Action<bool> onFinish)
        {
            BlazorMobileComponent.IsWebAssembly = true;

            if (app == null)
            {
                throw new NullReferenceException($"{nameof(IComponentsApplicationBuilder)} object is null");
            }

            InitCommon(onFinish);
        }

        internal static void InitServer(object appObject, Action<bool> onFinish)
        {
            BlazorMobileComponent.IsWebAssembly = false;

            if (appObject == null)
            {
                throw new NullReferenceException($"{nameof(BlazorService.ComponentEndpointConventionBuilder)} object is null");
            }

            InitCommon(onFinish);
        }
    }
}
