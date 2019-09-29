using System;
using Xamarin.Forms.Internals;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using BlazorMobile.Services;
using BlazorMobile.Components;
using ElectronNET.API;
using System.Collections.Generic;
using BlazorMobile.Interop;
using BlazorMobile.ElectronNET.Services;
using BlazorMobile.ElectronNET.Components;

namespace Xamarin.Forms
{
    public static class Forms
    {
        public static bool IsInitialized { get; private set; }

        public static void Init()
        {
            if (IsInitialized)
                return;
            IsInitialized = true;

            Log.Listeners.Add(new DelegateLogListener((c, m) => System.Diagnostics.Debug.WriteLine(m, c)));

            Device.SetIdiom(TargetIdiom.Desktop);
            Device.PlatformServices = new BlazorMobilePlatformServices();
            Device.Info = new BlazorMobileDeviceInfo();

            DependencyService.Register<IWebApplicationPlatform, ElectronWebApplicationPlatform>();

            WebApplicationFactory.Init();
        }

        public static HttpClientHandler HttpClientHandler { get; set; }

        class BlazorMobileDeviceInfo : DeviceInfo
        {
            public override Size PixelScreenSize => new Size(640, 480);

            public override Size ScaledScreenSize => PixelScreenSize;

            public override double ScalingFactor => 1;
        }

        internal class BlazorMobilePlatformServices : IPlatformServices, IDisposable
        {
            ~BlazorMobilePlatformServices()
            {
                Dispose(false);
            }

            public BlazorMobilePlatformServices()
            {
                MessagingCenter.Subscribe<Page, bool>(this, Page.BusySetSignalName, BusySetSignalNameHandler);
                MessagingCenter.Subscribe<Page, AlertArguments>(this, Page.AlertSignalName, AlertSignalNameHandler);
                MessagingCenter.Subscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName, ActionSheetSignalNameHandler);
            }

            public bool IsInvokeRequired => false;

            public string RuntimePlatform => "ElectronNET";

            public void BeginInvokeOnMainThread(Action action)
            {
                //It seem there is no notion of UI Thread nor non-background thread after testing
                //We can safely assume to queue a task with Task.Run
                Task.Run(action);
            }

            public Assembly[] GetAssemblies()
            {
                return AppDomain.CurrentDomain.GetAssemblies();
            }

            public string GetMD5Hash(string input)
            {
                throw new NotImplementedException();
            }

            public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
            {
                switch (size)
                {
                    default:
                    case NamedSize.Default:
                        return 16;
                    case NamedSize.Micro:
                        return 9;
                    case NamedSize.Small:
                        return 12;
                    case NamedSize.Medium:
                        return 22;
                    case NamedSize.Large:
                        return 32;
                }
            }


            public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage streamResponse = await client.GetAsync(uri.AbsoluteUri).ConfigureAwait(false);

                    if (!streamResponse.IsSuccessStatusCode)
                    {
                        Log.Warning("HTTP Request", $"Could not retrieve {uri}, status code {streamResponse.StatusCode}");
                        return null;
                    }

                    return await streamResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                }
            }

            public IIsolatedStorageFile GetUserStoreForApplication()
            {
                return new ElectronIsolatedStorage();
            }

            public void OpenUriAction(Uri uri)
            {
                Electron.Shell.OpenExternalAsync(uri.AbsoluteUri);
            }

            public void StartTimer(TimeSpan interval, Func<bool> callback)
            {
                Timer timer = null;
                timer = new Timer((_ => {
                    if (!callback())
                    {
                        timer?.Dispose();
                        timer = null;
                    }
                }), null, (int)interval.TotalMilliseconds, (int)interval.TotalMilliseconds);
            }

            public Ticker CreateTicker()
            {
                return new BlazorMobileTicker();
            }

            class BlazorMobileTicker : Ticker
            {
                Timer timer;
                protected override void DisableTimer()
                {
                    var t = timer;
                    timer = null;
                    t?.Dispose();
                }
                protected override void EnableTimer()
                {
                    if (timer != null)
                        return;
                    var interval = TimeSpan.FromSeconds(1.0 / 30);
                    timer = new Timer((_ => {
                        this.SendSignals();
                    }), null, (int)interval.TotalMilliseconds, (int)interval.TotalMilliseconds);
                }
            }

            public void QuitApplication()
            {
                Log.Warning(nameof(BlazorMobilePlatformServices), "Platform doesn't implement QuitApp");
            }

            public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
            {
                return new SizeRequest();
            }

            private bool _disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed) return;
                if (disposing)
                {
                    MessagingCenter.Unsubscribe<Page, AlertArguments>(this, "Xamarin.SendAlert");
                    MessagingCenter.Unsubscribe<Page, bool>(this, "Xamarin.BusySet");
                    MessagingCenter.Unsubscribe<Page, ActionSheetArguments>(this, "Xamarin.ShowActionSheet");
                }
                _disposed = true;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            void BusySetSignalNameHandler(Page sender, bool enabled)
            {
                //Noop
            }

            void AlertSignalNameHandler(Page sender, AlertArguments arguments)
            {
                //Managing buttons logic behavior with Xamarin.Forms
                List<string> buttons = new List<string>();
                if (!string.IsNullOrEmpty(arguments.Accept))
                {
                    buttons.Add(arguments.Accept);
                }
                buttons.Add(arguments.Cancel);

                Device.BeginInvokeOnMainThread(async () => {
                    var messageBoxResult = await Electron.Dialog.ShowMessageBoxAsync(new ElectronNET.API.Entities.MessageBoxOptions(arguments.Message)
                    {
                        Title = arguments.Title,
                        Buttons = buttons.ToArray(),
                        Type = ElectronNET.API.Entities.MessageBoxType.none,
                        NoLink = true
                    });

                    bool isOk = true;

                    //If buttons count equal 1, we only have a cancel button
                    //If we have more than 1 button but the Response index is the last button item, then it's a cancel button value
                    if (buttons.Count == 1 || buttons.Count > 1 && messageBoxResult.Response == buttons.Count - 1)
                    {
                        isOk = false;
                    }

                    arguments.SetResult(isOk);
                });
            }

            void ActionSheetSignalNameHandler(Page sender, ActionSheetArguments arguments)
            {
                //Managing buttons logic behavior with Xamarin.Forms
                List<string> buttons = new List<string>();

                if (string.IsNullOrEmpty(arguments.Destruction))
                {
                    //In mobile plateform, the Destruction button, if set, must be the top one
                    //We cannot colorize it
                    buttons.Add(arguments.Destruction);
                }

                //Adding selection
                buttons.AddRange(arguments.Buttons);

                //In ActionSheet, Cancel is always the last one
                buttons.Add(arguments.Cancel);

                string[] buttonsArray = buttons.ToArray();

                Device.BeginInvokeOnMainThread(async () => {
                    var messageBoxResult = await Electron.Dialog.ShowMessageBoxAsync(new ElectronNET.API.Entities.MessageBoxOptions(arguments.Title)
                    {
                        Buttons = buttonsArray,
                        Type = ElectronNET.API.Entities.MessageBoxType.question,
                        NoLink = false
                    });

                    arguments.SetResult(buttonsArray[messageBoxResult.Response]);
                });
            }

        }

        /// <summary>
        /// If your code already started your BlazorWebView.LaunchBlazorApp method, you should retrieve here the Electron main BrowserWindow used to create it.
        /// Otherwise, return a null Task value
        /// </summary>
        /// <returns></returns>
        public static Task<BrowserWindow> GetBrowserWindow()
        {
            var blazorWebView = BlazorWebViewFactory.GetMainElectronBlazorWebViewInstance();
            var electronBlazorWebView = blazorWebView as ElectronBlazorWebView;

            if (electronBlazorWebView == null)
                return Task.FromResult((BrowserWindow)null);

            return electronBlazorWebView.GetBrowserWindow();
        }

        /// <summary>
        /// Register the Xamarin.Forms Application class
        /// </summary>
        /// <param name="application"></param>
        public static void LoadApplication(BlazorApplication application)
        {
            Application.Current = application;
        }
    }
}