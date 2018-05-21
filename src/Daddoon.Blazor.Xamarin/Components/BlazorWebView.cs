using Daddoon.Blazor.Xam.Interop;
using Daddoon.Blazor.Xam.Services;
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

        public void LaunchBlazorApp()
        {
            switch (Device.RuntimePlatform)
            {
                //NOT WORKING: Blazor complaining about the current url being about:blank not matching with the rewritten base uri

                //case Device.UWP:
                //    //UWP does not support ScriptNotify if using a HTTP url. Using instead a HTML + BaseUrl as a workaround

                //    var htmlPageBytes = WebApplicationFactory.GetResource("index.html");
                //    var htmlPage = Encoding.UTF8.GetString(htmlPageBytes);
                //    htmlPageBytes = null;

                //    //HACK: Replacing base tag for UWP
                //    htmlPage = htmlPage.Replace(@"<base href=""/""", $@"<base href=""{WebApplicationFactory.GetBaseURL()}/""");

                //    Source = new HtmlWebViewSource()
                //    {
                //        Html = htmlPage
                //    };
                //    break;
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
            //TODO: See behavior(specially on Android) for Navigated event if we handle GoBack or GoForward

            if (blazorAppLaunched)
            {
                string content = ContextBridgeHelper.GetInjectableJavascript();
                Eval(content);
            }
        }
    }
}
