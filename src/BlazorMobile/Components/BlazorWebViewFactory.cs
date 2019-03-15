using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

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
                    return new BlazorGeckoView();
                default:
                    return new BlazorWebView();
            }
        }
    }
}
