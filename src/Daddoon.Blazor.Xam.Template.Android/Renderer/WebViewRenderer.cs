using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Daddoon.Blazor.Xam.Template.Droid.Renderer;
using Daddoon.Blazor.Xam.Template.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.WebView), typeof(BlazorWebViewRenderer))]
namespace Daddoon.Blazor.Xam.Template.Droid.Renderer
{
    public class WebViewClientExtended : WebViewClient
    {
        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private WebResourceResponse SendResponse(string mimeType, string encoding, int statusCode, string reasonPhrase, IDictionary<string, string> responseHeaders, Stream data)
        {
            var resp = new WebResourceResponse(mimeType, encoding, data);

            if (Android.OS.Build.VERSION.SdkInt > BuildVersionCodes.KitkatWatch)
            {
                resp.SetStatusCodeAndReasonPhrase(statusCode, reasonPhrase);
                resp.ResponseHeaders = responseHeaders;
            }

            return resp;
        }

        private WebResourceResponse ManageResponse (Android.Webkit.WebView view, string path)
        {
            var content = WebApplicationFactory.GetResourceStream(path);

            WebResourceResponse resp = null;

            //Content was not found
            if (content == null)
            {
                resp = SendResponse(
                    "text/plain",
                    "UTF8",
                    404,
                    "Not Found",
                    new Dictionary<string, string>() { { "Cache-Control", "no-cache" } },
                    GenerateStreamFromString("Page not found"));

                return resp;
            }

            resp = SendResponse(
            WebApplicationFactory.GetContentType(path),
            "UTF8",
            200,
            "OK",
            new Dictionary<string, string>() { { "Cache-Control", "no-cache" } },
            content);
            return resp;
        }

        [Obsolete]
        public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, string url)
        {
            var uri = new Uri(url, UriKind.Absolute);
            string path = uri.AbsolutePath;
            path = WebApplicationFactory.GetQueryPath(path);

            return ManageResponse(view, path);
        }

        public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, IWebResourceRequest request)
        {
            string path = WebApplicationFactory.GetQueryPath(request.Url.Path);
            return ManageResponse(view, path);
        }

        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, IWebResourceRequest request)
        {
            return true;
        }

        [Obsolete]
        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
        {
            return true;
        }
    }

    public class BlazorWebViewRenderer : WebViewRenderer
    {
        Context _context = null;

        public BlazorWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        private bool _init = false;
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);
            if (_init == false)
            {
                // perform initial setup
                var webView = Control;
                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.DomStorageEnabled = true;
                webView.Settings.DefaultTextEncodingName = "utf-8";
                webView.Settings.PluginsEnabled = true;
                webView.Settings.AllowContentAccess = true;
                webView.Settings.AllowUniversalAccessFromFileURLs = true;
                webView.Settings.AllowFileAccessFromFileURLs = true;
                webView.Settings.AllowFileAccess = true;
                webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;

                webView.SetWebChromeClient(new WebChromeClient());
                var webViewClient = new WebViewClientExtended();
                webView.SetWebViewClient(webViewClient);
                //webView.SetWebChromeClient(GetFormsWebChromeClient());

                SetNativeControl(webView);
                webView.Reload();
                _init = true;
            }
        }
    }
}