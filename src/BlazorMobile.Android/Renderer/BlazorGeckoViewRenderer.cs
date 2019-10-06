
using BlazorMobile.Components;
using BlazorMobile.Droid.Handler;
using BlazorMobile.Droid.Helper;
using BlazorMobile.Droid.Renderer;
using BlazorMobile.Droid.Services;
using BlazorMobile.Services;
using Org.Mozilla.Geckoview;
using System;
using Xam.Droid.GeckoView.Forms;
using Xam.Droid.GeckoView.Forms.Droid.Handlers;
using Xam.Droid.GeckoView.Forms.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BlazorGeckoView), typeof(BlazorGeckoViewRenderer))]
namespace BlazorMobile.Droid.Renderer
{
    public class BlazorGeckoViewRenderer : GeckoViewRenderer
    {
        internal void BlazorForwardSendNavigating(WebNavigatingEventArgs args)
        {
            Element.SendNavigating(args);
        }

        private bool _firstCall = true;

        public override Tuple<GeckoSession, GeckoRuntime> CreateNewSession(bool needSessionOpening, string uri)
        {
            if (!_firstCall)
            {
                GeckoSession newSession = new GeckoSession();
                GeckoRuntime newRuntime = GeckoRuntime.GetDefault(Context);
                
                //With our scenario this should not happen, but enforcing rule just for sanity check
                if (needSessionOpening)
                {
                    newSession.Open(newRuntime);
                }

                return new Tuple<GeckoSession, GeckoRuntime>(newSession, newRuntime);
            }

            var settings = new GeckoSessionSettings.Builder()
                .UsePrivateMode(true) //Use private mode in order to never cache anything at each app session
                .UseTrackingProtection(true)
                .UserAgentMode(GeckoSessionSettings.UserAgentModeMobile)
                .SuspendMediaWhenInactive(true)
                .AllowJavascript(true)
                .Build();

            GeckoSession _session = new GeckoSession(settings);
            GeckoRuntime _runtime = GeckoRuntime.GetDefault(Context);

            //Register BlazorMobile iframe listener WebExtension, as GeckoView LoadRequest does not bubble up when navigating through an iFrame.
            //NOTE: Delegate for WebExtension handling seem missing from current Xamarin.GeckoView generated bindings, but the handling will be workarounded through the local BlazorMobile server

            //WARNING: With our implementation, registering a WebExtension is per WebView if the same runtime object is used, not per session
            WebExtensionHelper.RegisterWebExtension(Element as BlazorGeckoView, _runtime, "resource://android/assets/obj/BlazorMobile/web_extensions/iframe_listener/");

            if (needSessionOpening)
            {
                _session.Open(_runtime);
            }

            _session.ProgressDelegate = new BlazorProgressDelegate(this);
            _session.ContentDelegate = new BlazorContentDelegate(this);
            _session.NavigationDelegate = new BlazorNavigationDelegate(this);

            if (WebApplicationFactory._debugFeatures)
            {
                _runtime.Settings.SetRemoteDebuggingEnabled(true);
                _runtime.Settings.SetConsoleOutputEnabled(true);
            }

            _firstCall = false;

            return Tuple.Create(_session, _runtime);
        }

        private KeyboardUtil keyboardHelper;

        protected override void OnElementChanged(ElementChangedEventArgs<GeckoViewForms> e)
        {
            if (Control == null)
            {
                _firstCall = true;
            }

            base.OnElementChanged(e);

            if (keyboardHelper == null)
            {
                keyboardHelper = new KeyboardUtil(BlazorWebViewService.GetCurrentActivity(), Control);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (keyboardHelper != null)
            {
                keyboardHelper.Disable();
            }

            base.Dispose();
        }
    }
}