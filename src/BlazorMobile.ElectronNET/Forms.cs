using System;
using Xamarin.Forms.Internals;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using BlazorMobile.Services;
using BlazorMobile.Components;
using System.Runtime.Serialization;


//This is inspired from Ooui.Forms
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

            WebApplicationFactory.Init();
        }

        public static HttpClientHandler HttpClientHandler { get; set; }

        class BlazorMobileDeviceInfo : DeviceInfo
        {
            public override Size PixelScreenSize => new Size(640, 480);

            public override Size ScaledScreenSize => PixelScreenSize;

            public override double ScalingFactor => 1;
        }

        class BlazorMobilePlatformServices : IPlatformServices
        {
            public bool IsInvokeRequired => false;

            public string RuntimePlatform => "ElectronNET";

            public void BeginInvokeOnMainThread(Action action)
            {
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
                //NOTE:  Wanted to use the same facility that ImageLoaderSourceHandler uses,
                //       but couldn't find an optional way to ignore certificate errors with self-signed 
                //       certificates with that approach.  Calling:
                //          ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;
                //       in web application seemed to get ignored.

                //var imageSource = new UriImageSource() { Uri = uri };
                //return imageSource.GetStreamAsync(cancellationToken);

                using (var client = HttpClientHandler == null ? new HttpClient() : new HttpClient(HttpClientHandler))
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
                throw new NotImplementedException();
            }

            public void OpenUriAction(Uri uri)
            {
                throw new NotImplementedException();
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
            }

            public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
            {
                return new SizeRequest();
            }
        }

        /// <summary>
        /// Register the Xamarin.Forms Application class
        /// </summary>
        /// <param name="application"></param>
        internal static void LoadApplication(BlazorApplication application)
        {
            Application.Current = application;
        }
    }
}