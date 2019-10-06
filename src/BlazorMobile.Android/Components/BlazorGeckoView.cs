using BlazorMobile.Common;
using BlazorMobile.Helper;
using BlazorMobile.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xam.Droid.GeckoView.Forms;
using Xamarin.Forms;

namespace BlazorMobile.Components
{
    public class BlazorGeckoView : GeckoViewForms, IBlazorWebView, IWebViewIdentity
    {
        private int _identity = -1;

        private EventHandler _onBlazorAppLaunched;
        event EventHandler IWebViewIdentity.OnBlazorAppLaunched
        {
            add
            {
                _onBlazorAppLaunched += value;
            }

            remove
            {
                _onBlazorAppLaunched -= value;
            }
        }

        void IWebViewIdentity.SendOnBlazorAppLaunched()
        {
            _onBlazorAppLaunched?.Invoke(this, EventArgs.Empty);
        }

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

        public void CallJSInvokableMethod(string assembly, string method, params object[] args)
        {
            WebViewHelper.CallJSInvokableMethod(assembly, method, args);
        }

        public void PostMessage<TArgs>(string messageName, TArgs args)
        {
            WebViewHelper.PostMessage(messageName, typeof(TArgs), new object[] { args });
        }
    }
}
