using System;
using System.Collections.Generic;
using Daddoon.Blazor.Xam.Common.Interop;
using Daddoon.Blazor.Xam.Components;
using Daddoon.Blazor.Xam.Interop;
using Daddoon.Blazor.Xam.Services;
using Daddoon.Blazor.Xam.UWP.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(BlazorWebView), typeof(BlazorWebViewRenderer))]
namespace Daddoon.Blazor.Xam.UWP.Renderer
{
    public class BlazorWebViewRenderer : WebViewRenderer
    {
        internal static void Init()
        {
            //Nothing to do, just force the compiler to not strip our component
        }

        private bool _init = false;

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);
            if (_init == false && Control != null)
            {
                Control.ScriptNotify += Control_ScriptNotify;
                _init = true;
            }
        }

        private void Control_ScriptNotify(object sender, Windows.UI.Xaml.Controls.NotifyEventArgs e)
        {
            ContextBridge.BridgeEvaluator((BlazorWebView)Element, e.Value);
        }
    }
}