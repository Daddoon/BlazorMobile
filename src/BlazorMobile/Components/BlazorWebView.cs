using BlazorMobile.Common.Helpers;
using BlazorMobile.Interop;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("BlazorMobile.UWP")]
namespace BlazorMobile.Components
{
    public class BlazorWebView : WebView, IBlazorWebView
    {
        internal BlazorWebView()
        {
            Navigated += BlazorWebView_Navigated;
            WebApplicationFactory.BlazorAppNeedReload += ReloadBlazorAppEvent;
        }

        private void ReloadBlazorAppEvent(object sender, EventArgs e)
        {
            LaunchBlazorApp();
        }

        internal static void InternalLaunchBlazorApp(IBlazorWebView webview)
        {
            webview.Source = new UrlWebViewSource()
            {
                Url = WebApplicationFactory.GetBaseURL()
            };
        }

        private void LaunchBlazorAppUri()
        {
            InternalLaunchBlazorApp(this);
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
            var webViewService = DependencyService.Get<IWebViewService>();
            webViewService.ClearCookies();

            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    //Giving some time on UWP, as it seem to fail to launch the new uri if called too soon
                    LaunchBlazorAppUri(1000);
                    break;
                default:
                    LaunchBlazorAppUri();
                    break;
            }
        }

        private void BlazorWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {

        }

        public View GetView()
        {
            return this;
        }
    }
}
