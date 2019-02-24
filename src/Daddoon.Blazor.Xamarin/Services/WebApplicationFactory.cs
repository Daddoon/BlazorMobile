using Daddoon.Blazor.Xam.Common.Interfaces;
using Daddoon.Blazor.Xam.Interop;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.CompilerServices;
using Waher.Networking.HTTP;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("Daddoon.Blazor.Xamarin.Android")]
[assembly: InternalsVisibleTo("Daddoon.Blazor.Xamarin.UWP")]
namespace Daddoon.Blazor.Xam.Services
{
    public static class WebApplicationFactory
    {
        private static HttpServer server;
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

        public static int GetHttpPort()
        {
            return HttpPort;
        }

        public static string GetBaseURL()
        {
            return $"http://localhost:{GetHttpPort()}";
        }

        internal static string GetQueryPath(string path)
        {
            path = WebUtility.UrlDecode(path.TrimStart('/'));

            if (string.IsNullOrEmpty(path))
            {
                //We are calling the index page. We must call index.html but hide it from url for Blazor routing.
                path = "index.html";
            }

            return path;
        }

        internal static void ManageRequest(IWebResponse response)
        {
            response.SetEncoding("UTF-8");

            string path = GetQueryPath(response.GetRequestedPath());

            var content = GetResourceStream(path);

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
            response.SetData(content);
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

        public static void StartWebServer()
        {
            Init(); //No-op if called twice

            //Request are managed by the WebView on Android
            if (Device.RuntimePlatform != Device.Android)
            {
                server = new HttpServer(HttpPort);
                server.Register(string.Empty, (req, resp) =>
                {
                    var response = new StdWebResponse(req, resp);
                    ManageRequest(response);
                }, true, true);
                return;
            }

            _isStarted = true;
        }

        public static void StopWebServer()
        {
            server?.Dispose();
            server = null;

            _isStarted = false;
        }
    }
}
