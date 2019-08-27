using BlazorMobile.Components;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BlazorMobile.ElectronNET.Components
{
    public class ElectronBlazorWebView : View, IBlazorWebView
    {
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

        public void LaunchBlazorApp()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
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
