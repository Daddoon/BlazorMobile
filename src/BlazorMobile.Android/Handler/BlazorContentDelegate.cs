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
using Org.Mozilla.Geckoview;
using Xam.Droid.GeckoView.Forms.Droid.Handlers;
using Xam.Droid.GeckoView.Forms.Droid.Renderers;

namespace BlazorMobile.Droid.Handler
{
    public class BlazorContentDelegate : ContentDelegate
    {
        public BlazorContentDelegate(GeckoViewRenderer renderer) : base(renderer)
        {

        }

        public override void OnCloseRequest(GeckoSession session)
        {
            base.OnCloseRequest(session);
        }
        public override void OnContextMenu(GeckoSession session, int screenX, int screenY, GeckoSession.ContentDelegateContextElement element)
        {
            base.OnContextMenu(session, screenX, screenY, element);
        }
        public override void OnCrash(GeckoSession session)
        {
            base.OnCrash(session);
        }

        public override void OnExternalResponse(GeckoSession session, GeckoSession.WebResponseInfo response)
        {
            base.OnExternalResponse(session, response);
        }
        public override void OnFirstComposite(GeckoSession session)
        {
            base.OnFirstComposite(session);
        }
        public override void OnFocusRequest(GeckoSession session)
        {
            base.OnFocusRequest(session);
        }
        public override void OnFullScreen(GeckoSession session, bool fullScreen)
        {
            base.OnFullScreen(session, fullScreen);
        }
        public override void OnTitleChange(GeckoSession session, string title)
        {
            base.OnTitleChange(session, title);
        }
    }
}