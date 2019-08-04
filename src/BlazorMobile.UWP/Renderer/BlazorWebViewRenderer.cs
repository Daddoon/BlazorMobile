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

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            //Override here in the future if we need some new behaviors
        }
    }
}