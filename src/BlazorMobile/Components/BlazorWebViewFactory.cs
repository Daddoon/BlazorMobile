using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("BlazorMobile.Android")]
namespace BlazorMobile.Components
{
    public static class BlazorWebViewFactory
    {
        /// <summary>
        /// As we can have some variation on the WebView renderer depending the platform,
        /// this automatically return the good type depending the current running platform
        /// </summary>
        /// <returns></returns>
        public static IBlazorWebView Create()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    return CreateBlazorGeckoViewInstance();
                default:
                    return new BlazorWebView();
            }
        }

        private static Type _geckoViewType = null;

        internal static void SetInternalBlazorGeckoViewType(Type geckoViewType)
        {
            _geckoViewType = geckoViewType;
        }

        internal static IBlazorWebView CreateBlazorGeckoViewInstance()
        {
            return (IBlazorWebView)Activator.CreateInstance(_geckoViewType);
        }
    }
}
