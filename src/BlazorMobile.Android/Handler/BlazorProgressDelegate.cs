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
    public class BlazorProgressDelegate : ProgressDelegate
    {
        public BlazorProgressDelegate(GeckoViewRenderer renderer) : base(renderer)
        {

        }

        public override void OnPageStart(GeckoSession session, string url)
        {
            base.OnPageStart(session, url);
        }
        public override void OnPageStop(GeckoSession session, bool success)
        {
            base.OnPageStop(session, success);
        }
        public override void OnProgressChange(GeckoSession session, int progress)
        {
            base.OnProgressChange(session, progress);

        }
        public override void OnSecurityChange(GeckoSession session, GeckoSession.ProgressDelegateSecurityInformation securityInfo)
        {
            base.OnSecurityChange(session, securityInfo);
        }
    }
}