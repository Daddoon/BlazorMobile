using BlazorMobile.Components;
using BlazorMobile.Helper;
using BlazorMobile.Interop;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BlazorMobile.ElectronNET.Components
{
    public class ElectronBlazorWebView : View, IBlazorWebView, IWebViewIdentity
    {
        private int _identity = -1;

        int IWebViewIdentity.GetWebViewIdentity()
        {
            return _identity;
        }

        ~ElectronBlazorWebView()
        {
            WebViewHelper.UnregisterWebView(this);
        }

        public ElectronBlazorWebView()
        {
            _identity = WebViewHelper.GenerateWebViewIdentity();
        }

        private const string noop = ": no-op on ElectronNET";

        public WebViewSource Source { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool CanGoBack { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool CanGoForward { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler<WebNavigatedEventArgs> Navigated;
        public event EventHandler<WebNavigatingEventArgs> Navigating;
        public event EventHandler<EvalRequested> EvalRequested;
        public event EvaluateJavaScriptDelegate EvaluateJavaScriptRequested;
        public event EventHandler GoBackRequested;
        public event EventHandler GoForwardRequested;
        public event EventHandler ReloadRequested;

        public void Eval(string script)
        {
            Console.WriteLine($"{nameof(Eval)}{noop}");
        }

        public Task<string> EvaluateJavaScriptAsync(string script)
        {
            Console.WriteLine($"{nameof(EvaluateJavaScriptAsync)}{noop}");
            return Task.FromResult(default(string));
        }

        public View GetView()
        {
            return this;
        }

        public void GoBack()
        {
            Console.WriteLine($"{nameof(GoBack)}{noop}");
        }

        public void GoForward()
        {
            Console.WriteLine($"{nameof(GoForward)}{noop}");
        }

        private bool _initialized = false;

        private BrowserWindow _browserWindow = null;

        public BrowserWindow GetBrowserWindow()
        {
            return _browserWindow;
        }

        public void LaunchBlazorApp()
        {
            if (_initialized)
            {
                return;
            }

            var createWindowTask = Task.Run(async () => _browserWindow = await Electron.WindowManager.CreateWindowAsync());
            Task.WaitAll(createWindowTask);

            var webPlatform = DependencyService.Get<IWebApplicationPlatform>();

            //We are just forcing the BaseURL caching before continuing
            webPlatform.GetBaseURL();

            _initialized = true;
        }

        public void Reload()
        {
            Console.WriteLine($"{nameof(Reload)}{noop}");
        }

        public void SendNavigated(WebNavigatedEventArgs args)
        {
            Console.WriteLine($"{nameof(SendNavigated)}{noop}");
        }

        public void SendNavigating(WebNavigatingEventArgs args)
        {
            Console.WriteLine($"{nameof(SendNavigating)}{noop}");
        }
    }
}
