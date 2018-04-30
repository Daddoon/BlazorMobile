using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Waher.Networking.HTTP;

namespace Daddoon.Blazor.Xam.Template.Services
{
    public static class WebApplicationFactory
    {
        private static HttpServer server;
        private static bool _isStarted = false;

        private static bool IsStarted()
        {
            return _isStarted;
        }

        private static Stream dataSource = null;
        private static ZipArchive archive = null;

        private static byte[] GetResource(string path)
        {
            MemoryStream data = new MemoryStream();
            if (dataSource == null)
            {
                dataSource = new MemoryStream(BlazorSources.app);
                archive = new ZipArchive(dataSource);
            }

            //Not the best approach in term of performance, will optimize further.
            //BTW everthing is then done locally
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.FullName.Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    entry.Open().CopyTo(data);
                    byte[] result = data.ToArray();
                    data.Dispose();
                    return data.ToArray();
                }
            }

            //If not found
            return null;
        }

        private static string GetContentType(string path)
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

        private const int HttpPort = 8282;

        public static int GetHttpPort()
        {
            return HttpPort;
        }

        public static string GetBaseURL()
        {
            return $"http://localhost:{GetHttpPort()}";
        }

        public static void StartWebServer()
        {
            server = new HttpServer(HttpPort);
            server.Register(string.Empty, (req, resp) =>
            {
                //req.SubPath doesn't return QueryString, so we don't have to do additional operations for ZIP search
                string path = WebUtility.UrlDecode(req.SubPath.TrimStart('/'));

                if (string.IsNullOrEmpty(path))
                {
                    //We are calling the index page. We must call index.html but hide it from url for Blazor routing.
                    path = "index.html";
                }

                byte[] content = GetResource(path);

                if (content == null)
                {
                    //Content not found
                    resp.StatusCode = 404;
                    return;
                }

                resp.StatusCode = 200;
                resp.ContentType = GetContentType(path);
                resp.Write(content);
            }, true, true);

            _isStarted = true;
        }

        public static void StopWebServer()
        {
            server?.Dispose();
            server = null;

            archive?.Dispose();
            archive = null;

            dataSource?.Dispose();
            dataSource = null;

            _isStarted = false;
        }
    }
}
