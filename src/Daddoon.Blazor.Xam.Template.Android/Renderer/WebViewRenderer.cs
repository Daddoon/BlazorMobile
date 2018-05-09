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
using Daddoon.Blazor.Xam.Template.Droid.Interop;
using Daddoon.Blazor.Xam.Template.Droid.Renderer;
using Daddoon.Blazor.Xam.Template.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.WebView), typeof(BlazorWebViewRenderer))]
namespace Daddoon.Blazor.Xam.Template.Droid.Renderer
{
    public class WebViewClientExtended : WebViewClient
    {
        private string GetBaseUrlFromWebResourceRequest(IWebResourceRequest request)
        {
            if (request == null)
                return null;

            string protocol = request.Url.Scheme;
            string host = request.Url.Host;
            int port = request.Url.Port;

            // if the port is not explicitly specified in the input, it will be -1.
            if (port == -1)
            {
                return $"{protocol}://{host}";
            }
            else
            {
                return $"{protocol}://{host}:{port}";
            }
        }

        public bool ShouldManageUrl(IWebResourceRequest request)
        {
            string url = GetBaseUrlFromWebResourceRequest(request);
            return ShouldManageUrl(url);
        }

        public bool ShouldManageUrl(string url)
        {
            if (string.IsNullOrEmpty(url) || !url.StartsWith(WebApplicationFactory.GetBaseURL()))
                return false;
            return true;
        }

        [Obsolete]
        public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, string url)
        {
            if (!ShouldManageUrl(url))
                return base.ShouldInterceptRequest(view, url);

            var response = new AndroidWebResponse(url);
            //Common management method
            WebApplicationFactory.ManageRequest(response);
            return response.GetWebResourceResponse();
        }

        public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, IWebResourceRequest request)
        {
            if (!ShouldManageUrl(request))
                return base.ShouldInterceptRequest(view, request);

            var response = new AndroidWebResponse(request);
            //Common management method
            WebApplicationFactory.ManageRequest(response);
            return response.GetWebResourceResponse();
        }

        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, IWebResourceRequest request)
        {
            if (!ShouldManageUrl(request))
                return base.ShouldOverrideUrlLoading(view, request);

            return true;
        }

        [Obsolete]
        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
        {
            if (!ShouldManageUrl(url))
                return base.ShouldOverrideUrlLoading(view, url);

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