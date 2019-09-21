using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Droid.Renderer;
using Org.Mozilla.Geckoview;
using Xam.Droid.GeckoView.Forms.Droid.Handlers;
using Xam.Droid.GeckoView.Forms.Droid.Renderers;
using Xamarin.Forms;

namespace BlazorMobile.Droid.Handler
{
    public class BlazorNavigationDelegate : NavigationDelegate
    {
        public BlazorNavigationDelegate(GeckoViewRenderer renderer) : base(renderer)
        {
        }

        public override void OnCanGoBack(GeckoSession session, bool canGoBack)
        {
            base.OnCanGoBack(session, canGoBack);
        }
        public override void OnCanGoForward(GeckoSession session, bool canGoForward)
        {
            base.OnCanGoForward(session, canGoForward);
        }

        public override GeckoResult OnLoadRequest(GeckoSession session, GeckoSession.avigationDelegateClassLoadRequest request)
        {
            return base.OnLoadRequest(session, request);
        }

        private List<GeckoSession> _notGCSessionList = new List<GeckoSession>();

        public override GeckoResult OnNewSession(GeckoSession session, string uri)
        {
            //Can assume result precisely here compared to Xamarin.Forms API
            var navEvent = WebNavigationEvent.NewPage;

            WebViewSource source;
            if (_renderer.Element != null && _renderer.Element.Source != null)
            {
                source = _renderer.Element.Source;
            }
            else
            {
                source = new UrlWebViewSource() { Url = uri };
            }

            var args = new WebNavigatingEventArgs(navEvent, source, uri);
            _renderer.Element.SendNavigating(args);
            
            if (args.Cancel)
            {
                return new GeckoResult(GeckoResult.FromValue(null));
            }
            else
            {
                //From documentation: A GeckoResult which holds the returned GeckoSession. May be null, in which case the request for a new window by web content will fail. e.g., window.open() will return null. The implementation of onNewSession is responsible for maintaining a reference to the returned object, to prevent it from being garbage collected.
                //See here: https://mozilla.github.io/geckoview/javadoc/mozilla-central/org/mozilla/geckoview/GeckoSession.NavigationDelegate.html#onNewSession-org.mozilla.geckoview.GeckoSession-java.lang.String-

                GeckoSession newSession = new GeckoSession();
                _notGCSessionList.Add(newSession);

                return new GeckoResult(GeckoResult.FromValue(newSession));
            }
        }
    }
}