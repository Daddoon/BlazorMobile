using System;
using System.Collections.Generic;
using System.ComponentModel;
using BlazorMobile.Common.Interop;
using BlazorMobile.Components;
using BlazorMobile.Interop;
using BlazorMobile.Services;
using BlazorMobile.UWP.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(BlazorWebView), typeof(BlazorWebViewRenderer))]
namespace BlazorMobile.UWP.Renderer
{
    public class BlazorWebViewRenderer : WebViewRenderer
    {
        internal static void Init()
        {
            //Nothing to do, just force the compiler to not strip our component
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            bool firstCall = false;
            if (Control == null)
            {
                //If null this is the first call
                firstCall = true;
            }

            base.OnElementChanged(e);

            if (firstCall)
            {
                Control.NewWindowRequested += Control_NewWindowRequested;
                Control.FrameNavigationStarting += Control_FrameNavigationStarting;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    Control.NewWindowRequested -= Control_NewWindowRequested;
                }
                catch (Exception)
                {
                }
            }

            base.Dispose(disposing);
        }

        private WebNavigatingEventArgs ManageRequestHandling(string uri, string referrerUri)
        {
            var navEvent = WebNavigationEvent.NewPage;
            WebViewSource source = new UrlWebViewSource() { Url = referrerUri };

            var eventArgs = new WebNavigatingEventArgs(navEvent, source, uri);

            ((IBlazorWebView)Element).SendNavigating(eventArgs);

            return eventArgs;
        }

        private void Control_FrameNavigationStarting(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationStartingEventArgs args)
        {
            string referrer = args.Uri.AbsoluteUri;

            if (Element != null && Element.Source != null && Element.Source is UrlWebViewSource)
            {
                referrer = ((UrlWebViewSource)Element.Source).Url;
            }

            var eventArgs = ManageRequestHandling(args.Uri.AbsoluteUri, referrer);

            args.Cancel = eventArgs.Cancel;
        }

        private void Control_NewWindowRequested(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNewWindowRequestedEventArgs args)
        {
            var eventArgs = ManageRequestHandling(args.Uri.AbsoluteUri, args.Referrer.AbsoluteUri);

            if (eventArgs.Cancel)
            {
                args.Handled = true;
            }
            else
            {
                args.Handled = false;
            }
        }
    }
}