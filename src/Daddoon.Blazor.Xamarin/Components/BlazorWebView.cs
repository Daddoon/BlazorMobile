using Daddoon.Blazor.Xam.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Components
{
    public class BlazorWebView : WebView
    {
        public BlazorWebView()
        {
            Navigated += BlazorWebView_Navigated;
        }

        private void BlazorWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            //TODO: See behavior (specially on Android) for Navigated event if we handle GoBack or GoForward

            string content = ContextBridgeHelper.GetInjectableJavascript();
            Eval(content);
        }
    }
}
