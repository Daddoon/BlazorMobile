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
        public static string RuntimePlatform { get; private set; } = Unknown;

        private static BlazorXamarinDeviceService xamService = null;

        private static void InitCommon(Action<bool> onFinish)
        {
            bool success = false;

            InternalHelper.SetTimeout(async () =>
            {
                xamService = new BlazorXamarinDeviceService();
                try
                {
                    var jsRuntime = BlazorXamarinExtensionScript.GetJSRuntime();

                    if (jsRuntime != null && await jsRuntime.InvokeAsync<bool>("BlazorXamarinRuntimeCheck"))
                    {
                        string resultRuntimePlatform = await xamService.GetRuntimePlatform();
                        RuntimePlatform = resultRuntimePlatform;
                    }
                    else
                    {
                        //If service return false or if JSRuntime is not yet ready (on server side scenario), we return the Browser default value
                        RuntimePlatform = Browser;
                    }

                    success = true;
                }
                catch (Exception ex)
                {
                    RuntimePlatform = Browser;
                }
                onFinish?.Invoke(success);
            }, 10);
        }

        internal static void Init(IComponentsApplicationBuilder app, string domElementSelector, Action<bool> onFinish)
        {
            if (app == null)
            {
                onFinish?.Invoke(false);
                throw new NullReferenceException($"{nameof(IComponentsApplicationBuilder)} object is null");
            }

            app.AddComponent<BlazorXamarinExtensionScript>(domElementSelector);

            InitCommon(onFinish);
        }

        internal static void InitServer(object appObject, string domElementSelector, Action<bool> onFinish)
        {
            if (appObject == null)
            {
                onFinish?.Invoke(false);
                throw new NullReferenceException($"{nameof(BlazorWebViewService.ComponentEndpointConventionBuilder)} object is null");
            }

            Type componentEndpointHelper = Type.GetType(BlazorWebViewService.ComponentEndpointConventionBuilderExtensionsGetType);
            MethodInfo method 
             = componentEndpointHelper.GetMethod("AddComponent", BindingFlags.Static | BindingFlags.Public);

            method.Invoke(null, new object[] { appObject, typeof(BlazorXamarinExtensionScript), domElementSelector });

            InitCommon(onFinish);
        }
    }
}
