using System;
using Daddoon.Blazor.Xam.Components;
using Daddoon.Blazor.Xam.Interop;
using Daddoon.Blazor.Xam.iOS.Renderer;
using Daddoon.Blazor.Xam.Services;
using Foundation;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BlazorWebView), typeof(BlazorWebViewRenderer))]
namespace Daddoon.Blazor.Xam.iOS.Renderer
{
    public class BlazorWebViewRenderer : ViewRenderer<WebView, WKWebView>, IWKScriptMessageHandler
    {
        /// <summary>
        /// Using an other method keyname as Init already exist on NSObject on iOS...
        /// </summary>
        internal static void BlazorInit()
        {
            //Nothing to do, just force the compiler to not strip our component
        }

        WKUserContentController userController;
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null && e.NewElement != null)
            {
                userController = new WKUserContentController();
                var script = new WKUserScript(new NSString(ContextBridgeHelper.GetInjectableJavascript(false)), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, "invokeAction");

                var config = new WKWebViewConfiguration { UserContentController = userController };
                var webView = new WKWebView(Frame, config);
                SetNativeControl(webView);
            }

            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
            }
            if (e.NewElement != null)
            {
                //Nothing to do
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            ContextBridge.BridgeEvaluator((BlazorWebView)Element, message.Body.ToString());
        }
    }

    public class NavigationDelegate : WKNavigationDelegate
    {
        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            // If navigation fails, this gets called
        }
    }
}