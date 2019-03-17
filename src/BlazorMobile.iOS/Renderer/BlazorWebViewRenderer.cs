using BlazorMobile.Common.Interop;
using BlazorMobile.Components;
using BlazorMobile.Interop;
using BlazorMobile.iOS.Renderer;
using BlazorMobile.Services;
using Foundation;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BlazorWebView), typeof(BlazorWebViewRenderer))]
namespace BlazorMobile.iOS.Renderer
{
    public class BlazorWebViewRenderer : ViewRenderer<BlazorWebView, WKWebView>, IWKScriptMessageHandler, IWebViewDelegate
    {
        /// <summary>
        /// Using an other method keyname as Init already exist on NSObject on iOS...
        /// </summary>
        internal static void BlazorInit()
        {
            //Nothing to do, just force the compiler to not strip our component
        }

        WKUserContentController userController;
        protected override void OnElementChanged(ElementChangedEventArgs<BlazorWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null && e.NewElement != null)
            {
                userController = new WKUserContentController();

                var config = new WKWebViewConfiguration { UserContentController = userController, Preferences = new WKPreferences()
                {
                    JavaScriptCanOpenWindowsAutomatically = false,
                    JavaScriptEnabled = true
                }
                };
                var webView = new WKWebView(Frame, config);
                webView.NavigationDelegate = new WebNavigationDelegate(this);
                SetNativeControl(webView);
            }

            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();

                var oldElementController = e.OldElement as IWebViewController;
                oldElementController.EvalRequested -= OnEvalRequested;
                oldElementController.EvaluateJavaScriptRequested -= OnEvaluateJavaScriptRequested;
                oldElementController.GoBackRequested -= OnGoBackRequested;
                oldElementController.GoForwardRequested -= OnGoForwardRequested;
                oldElementController.ReloadRequested -= OnReloadRequested;
            }
            if (e.NewElement != null)
            {
                var newElementController = e.NewElement as IWebViewController;
                newElementController.EvalRequested += OnEvalRequested;
                newElementController.EvaluateJavaScriptRequested += OnEvaluateJavaScriptRequested;
                newElementController.GoBackRequested += OnGoBackRequested;
                newElementController.GoForwardRequested += OnGoForwardRequested;
                newElementController.ReloadRequested += OnReloadRequested;
            }

            Load();
        }

        protected virtual void OnReloadRequested(object sender, EventArgs e)
        {
            Control.Reload();
        }

        protected virtual void OnGoForwardRequested(object sender, EventArgs e)
        {
            Control.GoForward();
        }

        protected virtual void OnGoBackRequested(object sender, EventArgs e)
        {
            Control.GoBack();
        }

        protected virtual Task<string> OnEvaluateJavaScriptRequested(string script)
        {
            throw new NotImplementedException($"{nameof(OnEvaluateJavaScriptRequested)}: Javascript evaluation is not yet reimplemented on the WKWebView Blazor renderer");
        }

        protected virtual void OnEvalRequested(object sender, EvalRequested e)
        {
            throw new NotImplementedException($"{nameof(OnEvalRequested)}: Javascript evaluation is not yet reimplemented on the WKWebView Blazor renderer");
        }

        protected virtual void Load()
        {
            WebViewSource source = Element.Source;
            UrlWebViewSource uri = source as UrlWebViewSource;

            if (uri != null)
            {
                Control.LoadRequest(new NSUrlRequest(new NSUrl(uri.Url)));
            }
            else
            {
                HtmlWebViewSource html = source as HtmlWebViewSource;
                if (html != null)
                {
                    Control.LoadHtmlString(html.Html, new NSUrl("/"));
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case "Source":
                    Load();
                    break;
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            //No-op
        }

        public void LoadHtml(string html, string baseUrl)
        {
            Control.LoadHtmlString(html, new NSUrl(baseUrl));
        }

        public void LoadUrl(string url)
        {
            Control.LoadRequest(new NSUrlRequest(new NSUrl(url)));
        }
    }

    public class WebNavigationDelegate : WKNavigationDelegate
    {

        private BlazorWebViewRenderer _renderer = null;

        public WebNavigationDelegate(BlazorWebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {

            var navType = navigationAction.NavigationType;
            var targetFrame = navigationAction.TargetFrame;

            var url = navigationAction.Request.Url;
            if (url.ToString().StartsWith("http") 
                && (targetFrame != null && targetFrame.MainFrame == true))
            {
                decisionHandler(WKNavigationActionPolicy.Allow);
            }
            else if ((url.ToString().StartsWith("http") && targetFrame == null)
                || url.ToString().StartsWith("mailto:")
                || url.ToString().StartsWith("tel:")) //Whatever your test happens to be
            {
                //Nothing to do actually, but we may expose theses kind of event to developer ?

            }
            else if (url.ToString().StartsWith("about"))
            {
                decisionHandler(WKNavigationActionPolicy.Allow);
            }
        }

        [Export("webView:didFinishNavigation:")]
        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
        }

        [Export("webView:didFailNavigation:withError:")]
        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            // If navigation fails, this gets called
            Console.WriteLine("DidFailNavigation:" + error.ToString());
        }

        [Export("webView:didFailProvisionalNavigation:withError:")]
        public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            // If navigation fails, this gets called
            Console.WriteLine("DidFailProvisionalNavigation" + error.ToString());
        }
    }
}