using BlazorMobile.Common.Helpers;
using BlazorMobile.Helper;
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
    public class BlazorWebView : WebView, IBlazorWebView, IWebViewIdentity
    {
        private int _identity = -1;

        bool IWebViewIdentity.BlazorAppLaunched { get; set; }

        int IWebViewIdentity.GetWebViewIdentity()
        {
            return _identity;
        }

        ~BlazorWebView()
        {
            WebViewHelper.UnregisterWebView(this);
            WebApplicationFactory.BlazorAppNeedReload -= ReloadBlazorAppEvent;
            WebApplicationFactory.EnsureBlazorAppLaunchedOrReload -= EnsureBlazorAppLaunchedOrReload;
        }

        public BlazorWebView()
        {
            WebApplicationFactory.BlazorAppNeedReload += ReloadBlazorAppEvent;
            WebApplicationFactory.EnsureBlazorAppLaunchedOrReload += EnsureBlazorAppLaunchedOrReload;

            _identity = WebViewHelper.GenerateWebViewIdentity();

            WebViewHelper.RegisterWebView(this);
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
