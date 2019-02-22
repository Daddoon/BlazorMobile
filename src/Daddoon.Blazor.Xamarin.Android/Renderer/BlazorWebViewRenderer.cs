using System;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Daddoon.Blazor.Xam.Components;
using Daddoon.Blazor.Xam.Droid.Interop;
using Daddoon.Blazor.Xam.Droid.Renderer;
using Daddoon.Blazor.Xam.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BlazorWebView), typeof(BlazorWebViewRenderer))]
namespace Daddoon.Blazor.Xam.Droid.Renderer
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class BlazorWebViewClient : WebViewClient
    {
        public BlazorWebViewClient()
        {

        }

        private BlazorWebView _parent;

        public BlazorWebViewClient(BlazorWebView parent) : this()
        {
            _parent = parent;
        }

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

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            _parent.OnNavigated();
            base.OnPageFinished(view, url);
        }
    }

    [Android.Runtime.Preserve(AllMembers = true)]
    public class BlazorWebViewRenderer : WebViewRenderer
    {
        internal static void Init()
        {
            //Nothing to do, just force the compiler to not strip our component
        }

        Context _context = null;

        public BlazorWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        private bool _init = false;
        protected override void OnElementChanged(ElementChangedEventArgs<global::Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);
            if (!_init)
            {
                //TODO: Maybe too much option enabled, must do some refactor and test here

                // perform initial setup
                var webView = Control;
                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.DomStorageEnabled = true;
                webView.Settings.DefaultTextEncodingName = "utf-8";
                webView.Settings.AllowContentAccess = true;
                webView.Settings.AllowUniversalAccessFromFileURLs = true;
                webView.Settings.AllowFileAccessFromFileURLs = true;
                webView.Settings.AllowFileAccess = true;

                #if DEBUG

                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Kitkat)
                {
                    Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
                }

                #endif

                //TODO: Calling SetWebChromeClient does crash the app after 30 sec
                //Something must be wrong here. As we don't have any additional requirement for the moment,
                //this will be stay as commented
                //webView.SetWebChromeClient(new BlazorWebChromeclient());

                var webViewClient = new BlazorWebViewClient((BlazorWebView)e.NewElement);
                webView.SetWebViewClient(webViewClient);
                SetNativeControl(webView);
                webView.Reload();
                _init = true;
            }

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("blazorxamarinJsBridge");
            }
            if (e.NewElement != null)
            {
                Control.AddJavascriptInterface(new JSBridge(this), "blazorxamarinJsBridge");
            }
        }
    }
}