using BlazorMobile.Interop;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;

namespace BlazorMobile.Components
{
    public class BlazorWebView : WebView, IBlazorWebView
    {
        public BlazorWebView()
        {
            Navigated += BlazorWebView_Navigated;
        }


        private void LaunchBlazorAppUri()
        {
            Source = new UrlWebViewSource()
            {
                Url = WebApplicationFactory.GetBaseURL()
            };
            blazorAppLaunched = true;
        }

        private void LaunchBlazorAppUri(int delayedMilliseconds)
        {
            Task.Run(async () => {
                await Task.Delay(delayedMilliseconds);
                Device.BeginInvokeOnMainThread(LaunchBlazorAppUri);
            });
        }

        public void LaunchBlazorApp()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    LaunchBlazorAppUri(1000);
                    break;
                default:
                    LaunchBlazorAppUri();
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
                        //string content = ContextBridgeHelper.GetInjectableJavascript();
                        //Eval(content);
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
