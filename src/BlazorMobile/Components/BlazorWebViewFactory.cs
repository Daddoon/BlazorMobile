using BlazorMobile.Common.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("BlazorMobile.Android")]
[assembly: InternalsVisibleTo("BlazorMobile.ElectronNET")]
namespace BlazorMobile.Components
{
    public static class BlazorWebViewFactory
    {
        //This is not a good thing about WebView disposing
        //This is temporary, see below
        private static IBlazorWebView _lastWebView = null;

        /// <summary>
        /// HACK: This method is just a temporary hack for GeckoView.
        /// GeckoView does not bubble up Iframe loading events compared to other browser
        /// As we are using a WebExtension instead to workaround this missing behavior, we
        /// need to forward the iframe Navigating Event to the BlazorWebView instance on Android.
        /// 
        /// As we don't yet manage to inject some WebView identity at Blazor app start to forward
        /// to a specific WebView object, we will just assume that the last BlazorWebView created
        /// object is the Webview we are looking for.
        /// </summary>
        /// <returns></returns>
        internal static IBlazorWebView GetLastBlazorWebViewInstance()
        {
            return _lastWebView;
        }

        /// <summary>
        /// Return an compatible WebView renderer for the current running platform
        /// </summary>
        /// <returns></returns>
        public static IBlazorWebView Create()
        {
            //ElectronNET case is managed separately as it is not on the Xamarin.Forms stack.
            if (ContextHelper.IsElectronNET())
            {
                return _lastWebView = CreateElectronBlazorWebViewInstance();
            }

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    return _lastWebView = CreateBlazorGeckoViewInstance();
                default:
                    return _lastWebView = new BlazorWebView();
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

        private static Type _electronWebviewType = null;

        internal static void SetInternalElectronBlazorWebView(Type electronWebviewType)
        {
            _electronWebviewType = electronWebviewType;
        }

        internal static IBlazorWebView CreateElectronBlazorWebViewInstance()
        {
            return (IBlazorWebView)Activator.CreateInstance(_electronWebviewType);
        }
    }
}
