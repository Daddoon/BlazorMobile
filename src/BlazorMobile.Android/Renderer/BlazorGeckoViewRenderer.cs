
using BlazorMobile.Components;
using BlazorMobile.Droid.Renderer;
using BlazorMobile.Services;
using Org.Mozilla.Geckoview;
using System;
using Xam.Droid.GeckoView.Forms.Droid.Handlers;
using Xam.Droid.GeckoView.Forms.Droid.Renderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BlazorGeckoView), typeof(BlazorGeckoViewRenderer))]
namespace BlazorMobile.Droid.Renderer
{
    public class BlazorGeckoViewRenderer : GeckoViewRenderer
    {
        public override Tuple<GeckoSession, GeckoRuntime> CreateNewSession()
        {
            var settings = new GeckoSessionSettings.Builder()
                .UsePrivateMode(true) //Use private mode in order to never cache anything at each app session
                .UseTrackingProtection(true)
                .UserAgentMode(GeckoSessionSettings.UserAgentModeMobile)
                .SuspendMediaWhenInactive(true)
                .AllowJavascript(true)
                .Build();

            GeckoSession _session = new GeckoSession(settings);
            GeckoRuntime _runtime = GeckoRuntime.Create(Context);
            _session.Open(_runtime);
            _session.ProgressDelegate = new ProgressDelegate(this);
            _session.ContentDelegate = new ContentDelegate(this);

            if (WebApplicationFactory._debugFeatures)
            {
                _runtime.Settings.SetRemoteDebuggingEnabled(true);
                _runtime.Settings.SetConsoleOutputEnabled(true);
            }

            return Tuple.Create(_session, _runtime);
        }
    }
}