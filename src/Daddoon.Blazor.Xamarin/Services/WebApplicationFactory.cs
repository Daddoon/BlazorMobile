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

        internal static MemoryStream GetResourceStream(string path)
        {
            if (_appResolver == null)
            {
                throw new NullReferenceException("The Blazor app resolver was not set! Please call WebApplicationFactory.RegisterAppStreamResolver method before launching your app");
            }

            using (Stream dataSource = _appResolver())
            {
                ZipArchive archive = null;
                archive = new ZipArchive(dataSource, ZipArchiveMode.Read);

                //data will contain found file output as stream
                MemoryStream data = new MemoryStream();
                ZipArchiveEntry entry = archive.GetEntry(path);

                if (entry != null)
                {
                    entry.Open().CopyTo(data);
                    data.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    data?.Dispose();
                    return null;
                }

                archive?.Dispose();

                return data;
            }
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

        private const int HttpPort = 8888;

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
