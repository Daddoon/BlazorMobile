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
using Java.IO;
using Java.Nio.Channels;
using Org.Mozilla.Geckoview;
using Xam.Droid.GeckoView.Forms.Droid.Handlers;
using Xam.Droid.GeckoView.Forms.Droid.Renderers;

namespace BlazorMobile.Droid.Handler
{
    public class BlazorGeckoViewPromptDelegate : Java.Lang.Object, GeckoSession.PromptDelegateClass
    {
        private GeckoViewRenderer _renderer;
        private bool _exceptionShouldThrow = false;

        public BlazorGeckoViewPromptDelegate(GeckoViewRenderer renderer, bool exceptionShouldThrow = false)
        {
            _renderer = renderer;
            _exceptionShouldThrow = exceptionShouldThrow;
        }

        public virtual void OnAlert(GeckoSession session, string title, string msg, GeckoSession.PromptDelegateClassAlertCallback callback)
        {
        }

        public virtual void OnAuthPrompt(GeckoSession session, string title, string msg, GeckoSession.romptDelegateClassAuthOptions options, GeckoSession.PromptDelegateClassAuthCallback callback)
        {
        }

        public virtual void OnButtonPrompt(GeckoSession session, string title, string msg, string[] btnMsg, GeckoSession.PromptDelegateClassButtonCallback callback)
        {
        }

        public virtual void OnChoicePrompt(GeckoSession session, string title, string msg, int type, GeckoSession.romptDelegateClassChoice[] choices, GeckoSession.PromptDelegateClassChoiceCallback callback)
        {
        }

        public virtual void OnColorPrompt(GeckoSession session, string title, string value, GeckoSession.PromptDelegateClassTextCallback callback)
        {
        }

        public virtual void OnDateTimePrompt(GeckoSession session, string title, int type, string value, string min, string max, GeckoSession.PromptDelegateClassTextCallback callback)
        {
        }

        public virtual void OnFilePrompt(GeckoSession session, string title, int type, string[] mimeTypes, GeckoSession.PromptDelegateClassFileCallback callback)
        {
            File myFile = new File(this.getFilesDir(), "temp.cube");

            try
            {
                FileInputStream @in = (FileInputStream)getContentResolver().openInputStream(data.getData());
                FileOutputStream @out = new FileOutputStream(myFile);
                FileChannel inChannel = @in.Channel;
                FileChannel outChannel = @out.Channel;
                inChannel.TransferTo(0, inChannel.Size(), outChannel);
                @in.Close();
                @out.Close();

                Android.Net.Uri uri = Android.Net.Uri.Parse("file:///" + myFile.AbsolutePath);
                callback.Confirm(_renderer.Context, uri);
            }
            catch (Exception e) {
                ConsoleHelper.WriteException(e);
                callback.Dismiss();
                if (_exceptionShouldThrow)
                {
                    throw;
                }
            }
        }

        public virtual GeckoResult OnPopupRequest(GeckoSession session, string targetUri)
        {
        }

        public virtual void OnTextPrompt(GeckoSession session, string title, string msg, string value, GeckoSession.PromptDelegateClassTextCallback callback)
        {
        }
    }
}