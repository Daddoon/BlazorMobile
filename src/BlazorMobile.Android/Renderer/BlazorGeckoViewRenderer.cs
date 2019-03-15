
using BlazorMobile.Components;
using BlazorMobile.Droid.Renderer;
using BlazorMobile.Services;
using Org.Mozilla.Geckoview;
using System;
using Xam.Droid.GeckoView.Forms.Droid.Renderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BlazorGeckoView), typeof(BlazorGeckoViewRenderer))]
namespace BlazorMobile.Droid.Renderer
{
    public class BlazorGeckoViewRenderer : GeckoViewRenderer
    {
        public override Tuple<GeckoSession, GeckoRuntime> CreateNewSession()
        {
            var resultTuple = base.CreateNewSession();

            if (WebApplicationFactory._debugFeatures)
            {
                resultTuple.Item2.Settings.SetRemoteDebuggingEnabled(true);
                resultTuple.Item2.Settings.SetConsoleOutputEnabled(true);
            }

            return resultTuple;
        }
    }
}