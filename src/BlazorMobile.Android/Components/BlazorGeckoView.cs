using BlazorMobile.Common;
using BlazorMobile.Helper;
using BlazorMobile.Services;
using System;
using Xam.Droid.GeckoView.Forms;
using Xamarin.Forms;

namespace BlazorMobile.Components
{
    public class BlazorGeckoView : GeckoViewForms, IBlazorWebView, IWebViewIdentity
    {
        private int _identity = -1;

        int IWebViewIdentity.GetWebViewIdentity()
        {
            return _identity;
        }

        ~BlazorGeckoView()
        {
            WebViewHelper.UnregisterWebView(this);
        }

        public BlazorGeckoView()
        {
            Navigated += BlazorWebView_Navigated;
            WebApplicationFactory.BlazorAppNeedReload += ReloadBlazorAppEvent;

            _identity = WebViewHelper.GenerateWebViewIdentity();
        }

        private void ReloadBlazorAppEvent(object sender, EventArgs e)
        {
            LaunchBlazorApp();
        }

        public void LaunchBlazorApp()
        {
            BlazorWebView.InternalLaunchBlazorApp(this);
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
