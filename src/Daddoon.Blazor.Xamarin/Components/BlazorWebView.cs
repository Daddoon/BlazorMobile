using Daddoon.Blazor.Xam.Interop;
using Daddoon.Blazor.Xam.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Components
{
    public class BlazorWebView : WebView, IBlazorWebView
    {
        public BlazorWebView()
        {
            Navigated += BlazorWebView_Navigated;
        }

        public void LaunchBlazorApp()
        {
            switch (Device.RuntimePlatform)
            {
                default:
                    Source = new UrlWebViewSource()
                    {
                        Url = WebApplicationFactory.GetBaseURL()
                    };
                    blazorAppLaunched = true;
                    break;
            }
        }

        private bool blazorAppLaunched = false;
        private void BlazorWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            OnNavigated();
        }

        public void OnNavigated()
        {
            if (blazorAppLaunched)
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        //WKWebview is wrapped over UIWebview. WkWebview has it's own delegate for this.
                        break;
                    default:
                        //TODO: We must verify that we are in the local URI context
                        string content = ContextBridgeHelper.GetInjectableJavascript();
                        Eval(content);
                        break;
                }
            }
        }

        public string GetReceiveEvaluator(string param)
        {
            param = JavascriptStringEscape(param);

            string javascriptEval = string.Empty;

            switch (Device.RuntimePlatform)
            {
                //case Device.Android:
                //    javascriptEval += "javascript: ";
                //    break;
                default:
                    break;
            }

            javascriptEval += $@"window.contextBridge.receive(""{param}"");";

            return javascriptEval;
        }

        public string JavascriptStringEscape(string source)
        {
            if (source == null)
                source = string.Empty;

            return HttpUtility.JavaScriptStringEncode(source);
        }

        public View GetView()
        {
            return this;
        }
    }
}
