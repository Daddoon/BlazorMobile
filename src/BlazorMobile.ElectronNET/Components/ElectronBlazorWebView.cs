using BlazorMobile.Common.Services;
using BlazorMobile.Components;
using BlazorMobile.ElectronNET.Helpers;
using BlazorMobile.ElectronNET.Services;
using BlazorMobile.Helper;
using BlazorMobile.Interop;
using BlazorMobile.Services;
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
        private const string MessageBrowserWindowNotAvailable = "BrowserWindow was not yet loaded";

        private IWebApplicationPlatform webAppPlaftorm = null;

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

        ~ElectronBlazorWebView()
        {
            WebViewHelper.UnregisterWebView(this);
        }

        public ElectronBlazorWebView()
        {
            _identity = WebViewHelper.GenerateWebViewIdentity();

            webAppPlaftorm = DependencyService.Get<IWebApplicationPlatform>();

            WebViewHelper.RegisterWebView(this);
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
            if (_browserWindow == null)
            {
                throw new InvalidOperationException(MessageBrowserWindowNotAvailable);
            }

            EvalRequested?.Invoke(this, new Xamarin.Forms.Internals.EvalRequested(script));

            Console.WriteLine($"{nameof(Eval)}{noop}");
        }

        public Task<string> EvaluateJavaScriptAsync(string script)
        {
            if (_browserWindow == null)
            {
                throw new InvalidOperationException(MessageBrowserWindowNotAvailable);
            }

            EvaluateJavaScriptRequested?.Invoke(script);

            Console.WriteLine($"{nameof(EvaluateJavaScriptAsync)}{noop}");
            return Task.FromResult(default(string));
        }

        public View GetView()
        {
            return this;
        }

        public void GoBack()
        {
            if (_browserWindow == null)
            {
                throw new InvalidOperationException(MessageBrowserWindowNotAvailable);
            }

            GoBackRequested?.Invoke(this, EventArgs.Empty);

            Console.WriteLine($"{nameof(GoBack)}{noop}");
        }

        public void GoForward()
        {
            if (_browserWindow == null)
            {
                throw new InvalidOperationException(MessageBrowserWindowNotAvailable);
            }

            GoForwardRequested?.Invoke(this, EventArgs.Empty);

            Console.WriteLine($"{nameof(GoForward)}{noop}");
        }

        private bool _initialized = false;

        private Task<BrowserWindow> getBrowserWindowTask = null;
        private BrowserWindow _browserWindow = null;

        public Task<BrowserWindow> GetBrowserWindow()
        {
            if (_browserWindow != null)
            {
                return Task.FromResult(_browserWindow);
            }
            else if (getBrowserWindowTask.IsCompleted)
            {
                return Task.FromResult(getBrowserWindowTask.Result);
            }
            else
            {
                return getBrowserWindowTask;
            }
        }

        private bool _firstLoad = true;

        private async Task<BrowserWindow> CreateMainBrowserWindow()
        {
            //This must be registered before window creation
            ((IWebViewIdentity)(this)).OnBlazorAppLaunched += ElectronBlazorWebView_OnBlazorAppLaunched;

            _browserWindow = await Electron.WindowManager.CreateWindowAsync();

            return _browserWindow;
        }

        private static bool _appDataCached = false;

        private void ElectronBlazorWebView_OnBlazorAppLaunched(object sender, EventArgs e)
        {
            if (!_appDataCached)
            {
                //Due to some race condition at ElectronNET startup, we set the value from Electron by an internal call
                //used before the OnBlazorAppLaunched event
                ((ElectronWebApplicationPlatform)webAppPlaftorm).SetCachedBaseURL(ElectronMetadata.BaseURL?.TrimEnd('/'));

                WebExtensionHelper.InstallOnNavigatingBehavior();

                _appDataCached = true;
            }
        }

        private string _previousURI = "about:blank";

        //This method is never called as there is a bug in current ElectronNET not firing the event
        //Just keeped here in case of fixing in the future
        private void WebContents_OnDidFinishLoad()
        {
            Task.Run(async () =>
            {
                string currentURI = await _browserWindow.WebContents.GetUrl();

                if (_firstLoad)
                {
                    //We are just forcing the BaseURL caching before continuing
                    ((ElectronWebApplicationPlatform)webAppPlaftorm).SetCachedBaseURL(currentURI);
                    _firstLoad = false;
                }

                //Get current URL and then forward it to Navigated event
                SendNavigated(new WebNavigatedEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource()
                {
                    Url = _previousURI
                }, currentURI, WebNavigationResult.Success));

                _previousURI = currentURI;
            });
        }

        public void LaunchBlazorApp()
        {
            if (_initialized)
            {
                return;
            }

            getBrowserWindowTask = Task.Run(CreateMainBrowserWindow);

            _initialized = true;
        }

        public void Reload()
        {
            if (_browserWindow == null)
            {
                throw new InvalidOperationException(MessageBrowserWindowNotAvailable);
            }

            ReloadRequested?.Invoke(this, EventArgs.Empty);

            _browserWindow.Reload();
        }

        public void SendNavigated(WebNavigatedEventArgs args)
        {
            Navigated?.Invoke(this, args);
        }

        public void SendNavigating(WebNavigatingEventArgs args)
        {
            Navigating?.Invoke(this, args);
        }

        public void CallJSInvokableMethod(string assembly,string method, params object[] args)
        {
            BlazorMobileService.SendMessageToJSInvokableMethod(assembly, method, args);
        }

        public void PostMessage<TArgs>(string messageName, TArgs args)
        {
            BlazorMobileService.SendMessageToSubscribers(messageName, typeof(TArgs), new object[] { args });
        }
    }
}
