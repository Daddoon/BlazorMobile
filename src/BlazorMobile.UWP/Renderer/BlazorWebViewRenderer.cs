using System;
using System.Collections.Generic;
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

        private bool _init = false;

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);
            if (_init == false && Control != null)
            {
                //Control.ScriptNotify += Control_ScriptNotify;
                _init = true;
            }
        }

        //private void Control_ScriptNotify(object sender, Windows.UI.Xaml.Controls.NotifyEventArgs e)
        //{
        //    ContextBridge.BridgeEvaluator((BlazorWebView)Element, e.Value);
        //}
    }
}