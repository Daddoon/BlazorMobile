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

        bool IWebViewIdentity.BlazorAppLaunched { get; set; }

        int IWebViewIdentity.GetWebViewIdentity()
        {
            return _identity;
        }

        ~BlazorGeckoView()
        {
            WebViewHelper.UnregisterWebView(this);
            WebApplicationFactory.BlazorAppNeedReload -= ReloadBlazorAppEvent;
            WebApplicationFactory.EnsureBlazorAppLaunchedOrReload -= EnsureBlazorAppLaunchedOrReload;
        }

        public BlazorGeckoView()
        {
            WebApplicationFactory.BlazorAppNeedReload += ReloadBlazorAppEvent;
            WebApplicationFactory.EnsureBlazorAppLaunchedOrReload += EnsureBlazorAppLaunchedOrReload;

            _identity = WebViewHelper.GenerateWebViewIdentity();
        }

        private void ReloadBlazorAppEvent(object sender, EventArgs e)
        {
            WebViewHelper.InternalLaunchBlazorApp(this, true);
        }

        private void EnsureBlazorAppLaunchedOrReload(object sender, EventArgs e)
        {
            if (!((IWebViewIdentity)this).BlazorAppLaunched)
            {
                ReloadBlazorAppEvent(sender, e);
            }
        }

        public void LaunchBlazorApp()
        {
            WebViewHelper.LaunchBlazorApp(this);
        }

        public View GetView()
        {
            return this;
        }
    }
}
