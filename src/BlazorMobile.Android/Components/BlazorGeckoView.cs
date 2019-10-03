using BlazorMobile.Common;
using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Serialization;
using BlazorMobile.Controller;
using BlazorMobile.Helper;
using BlazorMobile.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xam.Droid.GeckoView.Forms;
using Xamarin.Forms;

namespace BlazorMobile.Components
{
    public class BlazorGeckoView : GeckoViewForms, IBlazorWebView
    {
        public BlazorGeckoView()
        {
            Navigated += BlazorWebView_Navigated;
            WebApplicationFactory.BlazorAppNeedReload += ReloadBlazorAppEvent;
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

        public void CallJSInvokableMethod(string assembly, string method, params object[] args)
        {
            WebViewHelper.CallJSInvokableMethod(assembly, method, args);
        }

        public void PostMessage(string messageName, params object[] args)
        {
            WebViewHelper.PostMessage(messageName, args);
        }
    }
}
