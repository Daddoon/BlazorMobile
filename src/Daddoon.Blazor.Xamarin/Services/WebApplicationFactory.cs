using Daddoon.Blazor.Xam.Common.Interfaces;
using Daddoon.Blazor.Xam.Consts;
using Daddoon.Blazor.Xam.Controller;
using Daddoon.Blazor.Xam.Interop;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("Daddoon.Blazor.Xamarin.Android")]
[assembly: InternalsVisibleTo("Daddoon.Blazor.Xamarin.UWP")]
namespace Daddoon.Blazor.Xam.Services
{
    public static class WebApplicationFactory
    {
        private static WebServer server;
        private static CancellationTokenSource serverCts = new CancellationTokenSource();

        private static BlazorContextBridge blazorContextBridgeServer = null;

        private static bool _isStarted = false;

        private static bool IsStarted()
        {
            return _isStarted;
        }

        private static Func<Stream> _appResolver = null;

        /// <summary>
        /// Register how you get the Stream to your Blazor zipped application
        /// <para>For a performant unique entry point, getting a Stream from an</para>
        /// <para>Assembly manifest resource stream is recommended.</para>
        /// <para></para>
        /// <para>See <see cref="System.Reflection.Assembly.GetManifestResourceStream(string)"/></para>
        /// </summary>
        public static void RegisterAppStreamResolver(Func<Stream> resolver)
        {
            _appResolver = resolver;

            //Calling Init if not yet done. This will be a no-op if already called
            //We register init like this, as because of some linker problem with Xamarin,
            //we need to call the initializer from the "specialized project" (like Android)
            //that init itself and his components/renderer, then initializing this
            //
            //As iOS and UWP doesn't need a specific initialize at the moment but we may need
            //to call a generic init, the generic init is call in RegisterAppStream
            Init();
        }

        private static ZipArchive _zipArchive = null;
        private static object _zipLock = new object();
        private static ZipArchive GetZipArchive()
        {
            if (_zipArchive != null)
                return _zipArchive;

            //Stream is not disposed as it can be called in the future
            //Data source must be ideally loaded as a ressource like <see cref="Assembly.GetManifestResourceStream" /> for memory performance
            Stream dataSource = _appResolver();
            _zipArchive = new ZipArchive(dataSource, ZipArchiveMode.Read);

            return _zipArchive;
        }

        internal static MemoryStream GetResourceStream(string path)
        {
            if (_appResolver == null)
            {
                throw new NullReferenceException("The Blazor app resolver was not set! Please call WebApplicationFactory.RegisterAppStreamResolver method before launching your app");
            }

            MemoryStream data = null;

            lock (_zipLock)
            {
                try
                {
                    ZipArchive archive = GetZipArchive();

                    //Data will contain the found file, outputed as a stream
                    data = new MemoryStream();
                    ZipArchiveEntry entry = archive.GetEntry(path);

                    if (entry != null)
                    {
                        Stream entryStream = entry.Open();
                        entryStream.CopyTo(data);
                        entryStream.Dispose();
                        data.Seek(0, SeekOrigin.Begin);
                    }
                    else
                    {
                        data?.Dispose();
                        data = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
            return data;
        }

        internal static byte[] GetResource(string path)
        {
            MemoryStream content = GetResourceStream(path);
            if (content == null)
                return null;
            byte[] resultSet = content?.ToArray();
            content?.Dispose();

            return resultSet;
        }

        internal static string GetContentType(string path)
        {
            if (path.EndsWith(".wasm"))
            {
                return "application/wasm";
            }
            if (path.EndsWith(".dll"))
            {
                return "application/octet-stream";
            }

            //No critical mimetypes to check
            return MimeTypes.GetMimeType(path);
        }

        private const int DefaultHttpPort = 8888;

        private static int HttpPort = DefaultHttpPort;

        /// <summary>
        /// Define the HTTP port used for the webserver of your application.
        /// Default is 8888.
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool SetHttpPort(int port = DefaultHttpPort)
        {
            if (port <= 1024)
            {
                throw new InvalidOperationException("Cannot bind a port in the reserved port area !");
            }

            HttpPort = port;
            return true;
        }

        private static Func<string, string> _defaultPageDelegate;

        internal static Func<string, string> GetDefaultPageResultDelegate()
        {
            return _defaultPageDelegate;
        }

        public static void SetDefaultPageResult(Func<string, string> defaultPageDelegate)
        {
            _defaultPageDelegate = defaultPageDelegate;
        }

        public static int GetHttpPort()
        {
            return HttpPort;
        }

        private static string _localIP = null;
        private static string GetLocalWebServerIP()
        {
            if (_localIP == null)
            {
                //Some Android platform does not have a universal loopback address
                //This is intended to detect the Loopback entry point
                //Thanks to https://github.com/unosquare/embedio/issues/207#issuecomment-433437410
                var listener = new TcpListener(IPAddress.Loopback, 0);
                try
                {
                    listener.Start();
                    _localIP = ((IPEndPoint)listener.LocalEndpoint).Address.ToString();
                }
                catch
                {
                    _localIP = "localhost";
                }
                finally
                {
                    listener.Stop();
                }
            }

            return _localIP;
        }

        public static string GetBaseURL()
        {
            return $"http://{GetLocalWebServerIP()}:{GetHttpPort()}";
        }

        internal static string GetQueryPath(string path)
        {
            path = WebUtility.UrlDecode(path.TrimStart('/'));

            if (string.IsNullOrEmpty(path))
            {
                //We are calling the index page. We must call index.html but hide it from url for Blazor routing.
                path = Constants.DefaultPage;
            }

            return path;
        }

        internal static MemoryStream ManageIndexPageRendering(MemoryStream originalContent)
        {
            string indexContent = Encoding.UTF8.GetString(originalContent.ToArray());
            originalContent.Dispose();

            //Do user logic
            var userDelegate = GetDefaultPageResultDelegate();
            if (userDelegate != null)
            {
                indexContent = userDelegate(indexContent);
            }

            //Do BlazorContextBridgeLogic
            //Get content to INJECT
            string injectableContent = ContextBridgeHelper.GetInjectableJavascript(false).Replace("%_contextBridgeURI%", GetContextBridgeURI());
            indexContent = indexContent.Replace("</blazorXamarin>", "</blazorXamarin>\r\n<script type=\"application/javascript\">" + injectableContent + "\r\n</script>\r\n\r\n");

            return new MemoryStream(Encoding.UTF8.GetBytes(indexContent));
        }

        internal static async Task ManageRequest(IWebResponse response)
        {
            response.SetEncoding("UTF-8");

            string path = GetQueryPath(response.GetRequestedPath());

            var content = GetResourceStream(path);

            //Manage Index content
            if (path == Constants.DefaultPage)
            {
                content = ManageIndexPageRendering(content);
            }

            response.AddResponseHeader("Cache-Control", "no-cache");
            response.AddResponseHeader("Access-Control-Allow-Origin", GetBaseURL());

            if (content == null)
            {
                //Content not found
                response.SetStatutCode(404);
                response.SetReasonPhrase("Not found");
                response.SetMimeType("text/plain");
                return;
            }

            response.SetStatutCode(200);
            response.SetReasonPhrase("OK");
            response.SetMimeType(GetContentType(path));
            await response.SetDataAsync(content);
        }

        private static bool _firstCall = true;

        /// <summary>
        /// Init the WebApplicationFactory with the given app stream resolver.
        /// Shorthand for <see cref="WebApplicationFactory.RegisterAppStreamResolver" />
        /// </summary>
        /// <param name="appStreamResolver"></param>
        internal static void Init(Func<Stream> appStreamResolver)
        {
            RegisterAppStreamResolver(appStreamResolver);
        }

        internal static void Init()
        {
            if (_firstCall)
            {
                //Register IBlazorXamarinDeviceService for getting base metadata for Blazor
                DependencyService.Register<IBlazorXamarinDeviceService, BlazorXamarinDeviceService>();

                //Do something in the future if required
                _firstCall = false;
            }
        }

        /// <summary>
        /// As test for the moment. 10 seconds max
        /// </summary>
        private const int NativeSocketTimeout = 1000;

        private const string _contextBridgeRelativeURI = "/contextBridge";

        internal static string GetContextBridgeURI()
        {
            return GetBaseURL().Replace("http://", "ws://") + _contextBridgeRelativeURI;
        }

        internal static BlazorContextBridge GetBlazorContextBridgeServer()
        {
            return blazorContextBridgeServer;
        }

        internal static bool _debugFeatures = false;

        /// <summary>
        /// Add additionals features like Browser console.log output on logcat on Android
        /// </summary>
        public static void EnableDebugFeatures()
        {
            _debugFeatures = true;
        }

        public static void StartWebServer()
        {
            Init(); //No-op if called twice

            if (IsStarted())
            {
                //Already started
                return;
            }

            //EmbedIO test
            server = new WebServer(GetBaseURL(), RoutingStrategy.Regex);
            server.WithLocalSession();

            server.RegisterModule(new WebApiModule());
            server.Module<WebApiModule>().RegisterController<BlazorController>();

            //Reference to the BlazorContextBridge Websocket service
            blazorContextBridgeServer = new BlazorContextBridge();

            server.RegisterModule(new WebSocketsModule());
            server.Module<WebSocketsModule>().RegisterWebSocketsServer(_contextBridgeRelativeURI, blazorContextBridgeServer);

            serverCts = new CancellationTokenSource();

            Task.Factory.StartNew(async () =>
            {
                Console.WriteLine("Starting Server...");
                await server.RunAsync(serverCts.Token);
            });
        }

        public static void StopWebServer()
        {
            //In order to stop the waiting background thread
            try
            {
                serverCts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(WebApplicationFactory)}.{nameof(WebApplicationFactory.StopWebServer)} - {nameof(serverCts)}: {ex.Message}");
            }

            try
            {
                if (blazorContextBridgeServer != null)
                {
                    blazorContextBridgeServer.Dispose();
                    blazorContextBridgeServer = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(WebApplicationFactory)}.{nameof(WebApplicationFactory.StopWebServer)} - {nameof(blazorContextBridgeServer)}: {ex.Message}");
            }

            server?.Dispose();
            server = null;

            _isStarted = false;
        }
    }
}
