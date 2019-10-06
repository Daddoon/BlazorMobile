using BlazorMobile.Common.Helpers;
using BlazorMobile.Components;
using BlazorMobile.iOS.Renderer;
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

        internal WebNavigationEvent _lastBackForwardEvent;

        internal bool _ignoreSourceChanges;

        private WKWebView webView;

        internal WKWebView GetNativeWebView()
        {
            return webView;
        }

        WKUserContentController userController;
        protected override void OnElementChanged(ElementChangedEventArgs<BlazorWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
               userController = new WKUserContentController();

                var config = new WKWebViewConfiguration
                {
                    UserContentController = userController,
                    Preferences = new WKPreferences()
                    {
                        JavaScriptCanOpenWindowsAutomatically = false,
                        JavaScriptEnabled = true
                    },
                    WebsiteDataStore = WKWebsiteDataStore.NonPersistentDataStore
                };

                webView = new WKWebView(Frame, config);
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
                ((WebNavigationDelegate)webView.NavigationDelegate).SetBlazorWebViewForms(null);
            }
            if (e.NewElement != null)
            {
                var newElementController = e.NewElement as IWebViewController;
                newElementController.EvalRequested += OnEvalRequested;
                newElementController.EvaluateJavaScriptRequested += OnEvaluateJavaScriptRequested;
                newElementController.GoBackRequested += OnGoBackRequested;
                newElementController.GoForwardRequested += OnGoForwardRequested;
                newElementController.ReloadRequested += OnReloadRequested;
                currentViewForms = e.NewElement;
                ((WebNavigationDelegate)webView.NavigationDelegate).SetBlazorWebViewForms(e.NewElement);
            }

            Load();
        }

        private BlazorWebView currentViewForms = null;

        internal void UpdateCanGoBackForward()
        {
            if (currentViewForms == null)
            {
                return;
            }

            ((IWebViewController)currentViewForms).CanGoBack = Control.CanGoBack;
            ((IWebViewController)currentViewForms).CanGoForward = Control.CanGoForward;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            //Fix incorrect bounds/safe area with WkWebview when changing orientation
            webView.Frame = UIKit.UIScreen.MainScreen.Bounds;
        }

        protected virtual void OnReloadRequested(object sender, EventArgs e)
        {
            Control.Reload();
        }

        void OnGoBackRequested(object sender, EventArgs eventArgs)
        {
            if (Control.CanGoBack)
            {
                _lastBackForwardEvent = WebNavigationEvent.Back;
                Control.GoBack();
            }

            UpdateCanGoBackForward();
        }

        void OnGoForwardRequested(object sender, EventArgs eventArgs)
        {
            if (Control.CanGoForward)
            {
                _lastBackForwardEvent = WebNavigationEvent.Forward;
                Control.GoForward();
            }

            UpdateCanGoBackForward();
        }

        async Task<string> OnEvaluateJavaScriptRequested(string script)
        {
            var result = await Control.EvaluateJavaScriptAsync(script);
            return result?.ToString();
        }

        void OnEvalRequested(object sender, EvalRequested eventArg)
        {
            Control.EvaluateJavaScriptAsync(eventArg.Script);
        }

        protected virtual void Load()
        {
            if (_ignoreSourceChanges)
                return;

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

            UpdateCanGoBackForward();
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

        WebNavigationEvent _lastEvent;

        public WebNavigationDelegate(BlazorWebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        private BlazorWebView _blazorWebViewForms = null;

        public void SetBlazorWebViewForms(BlazorWebView blazorWebViewForms)
        {
            _blazorWebViewForms = blazorWebViewForms;
        }

        [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            //Should not happen
            if (_blazorWebViewForms == null)
            {
                decisionHandler(WKNavigationActionPolicy.Cancel);
            }
            else
            {
                var navEvent = WebNavigationEvent.NewPage;
                var navigationType = navigationAction.NavigationType;
                switch (navigationType)
                {
                    case WKNavigationType.LinkActivated:
                        navEvent = WebNavigationEvent.NewPage;
                        break;
                    case WKNavigationType.FormSubmitted:
                        navEvent = WebNavigationEvent.NewPage;
                        break;
                    case WKNavigationType.BackForward:
                        navEvent = _renderer._lastBackForwardEvent;
                        break;
                    case WKNavigationType.Reload:
                        navEvent = WebNavigationEvent.Refresh;
                        break;
                    case WKNavigationType.FormResubmitted:
                        navEvent = WebNavigationEvent.NewPage;
                        break;
                    case WKNavigationType.Other:
                        navEvent = WebNavigationEvent.NewPage;
                        break;
                }

                _lastEvent = navEvent;
                var request = navigationAction.Request;
                var lastUrl = request.Url.ToString();

                WebViewSource source;
                if (_renderer.Element != null && _renderer.Element.Source != null)
                {
                    source = _renderer.Element.Source;
                }
                else
                {
                    source = new UrlWebViewSource() { Url = lastUrl };
                }

                var args = new WebNavigatingEventArgs(navEvent, source, lastUrl);

                _blazorWebViewForms.SendNavigating(args);
                _renderer.UpdateCanGoBackForward();
                decisionHandler(args.Cancel ? WKNavigationActionPolicy.Cancel : WKNavigationActionPolicy.Allow);
            }
        }

        string GetCurrentUrl()
        {
            return _renderer?.GetNativeWebView()?.Url?.AbsoluteUrl?.ToString();
        }

        [Export("webView:didFinishNavigation:")]
        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            if (webView.IsLoading)
                return;

            var url = GetCurrentUrl();
            if (url == $"file://{NSBundle.MainBundle.BundlePath}/")
                return;

            _renderer._ignoreSourceChanges = true;
            _blazorWebViewForms.SetValueFromRenderer(WebView.SourceProperty, new UrlWebViewSource { Url = url });
            _renderer._ignoreSourceChanges = false;

            var args = new WebNavigatedEventArgs(_lastEvent, _blazorWebViewForms.Source, url, WebNavigationResult.Success);
            _blazorWebViewForms?.SendNavigated(args);

            _renderer.UpdateCanGoBackForward();
        }

        [Export("webView:didFailNavigation:withError:")]
        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            var url = GetCurrentUrl();
            _blazorWebViewForms?.SendNavigated(
                new WebNavigatedEventArgs(_lastEvent, new UrlWebViewSource { Url = url }, url, WebNavigationResult.Failure)
            );

            _renderer.UpdateCanGoBackForward();
        }

        [Export("webView:didFailProvisionalNavigation:withError:")]
        public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
        }
    }
}