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

        public override GeckoResult OnNewSession(GeckoSession session, string uri)
        {
            return base.OnNewSession(session, uri);
        }
    }
}