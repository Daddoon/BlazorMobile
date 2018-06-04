using Daddoon.Blazor.Xam.Components;
using Daddoon.Blazor.Xam.Interop;
using Daddoon.Blazor.Xam.iOS.Renderer;
using Daddoon.Blazor.Xam.Services;
using Foundation;
using System;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BlazorWebView), typeof(BlazorWebViewRenderer))]
namespace Daddoon.Blazor.Xam.iOS.Renderer
{
    public class BlazorWebViewRenderer : ViewRenderer<BlazorWebView, WKWebView>, IWKScriptMessageHandler
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
                //var script = new WKUserScript(new NSString(ContextBridgeHelper.GetInjectableJavascript(false)), WKUserScriptInjectionTime.AtDocumentEnd, false);
                //userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, "invokeAction");

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
                userController.RemoveScriptMessageHandler("invokeAction");
            }
            if (e.NewElement != null)
            {
                Control.LoadRequest(new NSUrlRequest(new NSUrl(WebApplicationFactory.GetBaseURL())));
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            ContextBridge.BridgeEvaluator(Element, message.Body.ToString(), delegate (string result)
            {
                Control.EvaluateJavaScript((NSString)result, GetJavascriptEvaluationResultHandler());
            });
        }

        private WKJavascriptEvaluationResult _handler = null;
        public WKJavascriptEvaluationResult GetJavascriptEvaluationResultHandler()
        {
            if (_handler == null)
            {
                _handler = (NSObject result, NSError err) =>
                {
                    if (err != null)
                    {
                        System.Console.WriteLine(err);
                    }
                    if (result != null)
                    {
                        System.Console.WriteLine(result);
                    }
                };
            }

            return _handler;
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
            var content = ContextBridgeHelper.GetInjectableJavascript();
            var handler = _renderer.GetJavascriptEvaluationResultHandler();

            _renderer.Control.EvaluateJavaScript((NSString)content, handler);
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