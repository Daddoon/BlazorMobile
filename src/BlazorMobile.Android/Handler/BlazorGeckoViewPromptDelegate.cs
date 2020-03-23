using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Droid.Helper;
using BlazorMobile.Droid.Services;
using Java.IO;
using Java.Nio.Channels;
using Org.Mozilla.Geckoview;
using Xam.Droid.GeckoView.Forms.Droid.Handlers;
using Xam.Droid.GeckoView.Forms.Droid.Renderers;
using Xamarin.Forms;

namespace BlazorMobile.Droid.Handler
{
    internal class BlazorFileDialogDismissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
    {
        Action _onDismiss = null;

        public void SetDismissHandler(System.Action onDismiss)
        {
            _onDismiss = onDismiss;
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            _onDismiss?.Invoke();
        }
    }

    public class BlazorGeckoViewPromptDelegate : Java.Lang.Object, GeckoSession.PromptDelegateClass
    {
        private static readonly Int32 REQUEST_CAMERA_TEST = 8;
        private static readonly Int32 SELECT_FILE = 9;

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
            var currentActivity = BlazorWebViewService.GetCurrentActivity();

            //TODO: Localize text inputs

            string[] items = { "Take Photo", "Choose from Library", "Cancel" };
            string _imageUri;

            Action showActionsDialog = null;

            showActionsDialog = () =>
            {

                using (var dialogBuilder = new Android.App.AlertDialog.Builder(_renderer.Context))
                {
                    BlazorFileDialogDismissListener onDismissEvent = new BlazorFileDialogDismissListener();
                    bool shouldDismiss = true;
                    onDismissEvent.SetDismissHandler(() =>
                    {
                        if (shouldDismiss)
                        {
                            callback.Dismiss();
                        }
                    });

                    dialogBuilder.SetOnDismissListener(onDismissEvent);
                    dialogBuilder.SetTitle("Add Photo");
                    dialogBuilder.SetItems(items, (d, args) =>
                    {
                        //We set this value to false as we are in a callback
                        //Every action of the callback will have to manage the dismiss of the file input with this value to false
                        //This is made in order to Dismiss on the fine input callback if the user use a back event on his device instead
                        shouldDismiss = false;

                        //Take photo
                        if (args.Which == 0)
                        {
                            RequestPermissionHelper.CheckForPermission(new[] {
                            Android.Manifest.Permission.WriteExternalStorage,
                            Android.Manifest.Permission.Camera
                                }, () =>
                            {
                                bool isMounted = Android.OS.Environment.ExternalStorageState == Android.OS.Environment.MediaMounted;

                                var uri = currentActivity.ContentResolver.Insert(isMounted ? MediaStore.Images.Media.ExternalContentUri
                                : MediaStore.Images.Media.InternalContentUri, new ContentValues());
                                _imageUri = uri.ToString();
                                var i = new Intent(MediaStore.ActionImageCapture);
                                i.PutExtra(MediaStore.ExtraOutput, uri);
                                currentActivity.StartActivityForResult(i, REQUEST_CAMERA_TEST);

                                //TODO
                                //TO REMOVE AFTER DEV
                                callback.Dismiss();

                            }, () => showActionsDialog()); //If denied, we must show the previous window again

                        }
                        //Choose from gallery
                        else if (args.Which == 1)
                        {
                            RequestPermissionHelper.CheckForPermission(Android.Manifest.Permission.ReadExternalStorage, () =>
                            {
                                var intent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                                intent.SetType("image/*");
                                currentActivity.StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), SELECT_FILE);

                                //TODO
                                //TO REMOVE AFTER DEV
                                callback.Dismiss();

                            }, () => showActionsDialog()); //If denied, we must show the previous window again
                        }
                        else
                        {
                            callback.Dismiss();
                        }
                    });

                    dialogBuilder.Show();
                }
            };

            showActionsDialog();

            //File myFile = new File(this.getFilesDir(), "temp.cube");

            //try
            //{
            //    FileInputStream @in = (FileInputStream)getContentResolver().openInputStream(data.getData());
            //    FileOutputStream @out = new FileOutputStream(myFile);
            //    FileChannel inChannel = @in.Channel;
            //    FileChannel outChannel = @out.Channel;
            //    inChannel.TransferTo(0, inChannel.Size(), outChannel);
            //    @in.Close();
            //    @out.Close();

            //    Android.Net.Uri uri = Android.Net.Uri.Parse("file:///" + myFile.AbsolutePath);
            //    callback.Confirm(_renderer.Context, uri);
            //}
            //catch (Exception e) {
            //    ConsoleHelper.WriteException(e);
            //    callback.Dismiss();
            //    if (_exceptionShouldThrow)
            //    {
            //        throw;
            //    }
            //}
        }

        public virtual GeckoResult OnPopupRequest(GeckoSession session, string targetUri)
        {
            //TODO
            throw new NotImplementedException();
        }

        public virtual void OnTextPrompt(GeckoSession session, string title, string msg, string value, GeckoSession.PromptDelegateClassTextCallback callback)
        {
        }
    }
}