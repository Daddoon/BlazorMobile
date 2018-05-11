using System;

using Daddoon.Blazor.Xam.Components;
using Daddoon.Blazor.Xam.Interop;
using Daddoon.Blazor.Xam.UWP.Renderer;
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

        public BlazorWebViewRenderer() : base()
        {
            Control.ScriptNotify += Control_ScriptNotify;

            _init = true;
        }

        private async void Control_ScriptNotify(object sender, Windows.UI.Xaml.Controls.NotifyEventArgs e)
        {
            //TODO
            //throw new NotImplementedException();
        }
    }
}