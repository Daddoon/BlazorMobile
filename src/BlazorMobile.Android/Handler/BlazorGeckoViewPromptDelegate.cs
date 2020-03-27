using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Droid.Helper;
using BlazorMobile.Droid.Services;
using Java.IO;
using Java.Lang;
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

    /// <summary>
    /// https://mozilla.github.io/geckoview/javadoc/mozilla-central/constant-values.html#org.mozilla.geckoview.GeckoSession.PromptDelegate.FilePrompt.Type.MULTIPLE
    /// </summary>
    public static class FilePromptType
    {
        public const int SINGLE = 1;
        public const int MULTIPLE = 2;
    }

    public class ModifiableChoice
    {
        public bool ModifiableSelected { get; set; }

        public string ModifiableLabel { get; set; }

        public GeckoSession.romptDelegateClassChoice Choice { get; set; }

        public ModifiableChoice(GeckoSession.romptDelegateClassChoice c)
        {
            Choice = c;
            ModifiableSelected = Choice.Selected;
            ModifiableLabel = Choice.Label;
        }
    }

    public class ModifiableChoiceArrayAdapter<T> : ArrayAdapter<T> where T : ModifiableChoice
    {
        private static int TYPE_MENU_ITEM = 0;
        private static int TYPE_MENU_CHECK = 1;
        private static int TYPE_SEPARATOR = 2;
        private static int TYPE_GROUP = 3;
        private static int TYPE_SINGLE = 4;
        private static int TYPE_MULTIPLE = 5;
        private static int TYPE_COUNT = 6;

        public int _choicePromptType;
        Android.App.AlertDialog.Builder _builder;
        Android.Widget.ListView _list;

        public ModifiableChoiceArrayAdapter(Context context, int resource, int choicePromptType, Android.App.AlertDialog.Builder builder, Android.Widget.ListView list) : base(context, resource)
        {
            _choicePromptType = choicePromptType;
            _builder = builder;
            _list = list;
        }

        private LayoutInflater mInflater;
        private Android.Views.View mSeparator;

        public override int ViewTypeCount => TYPE_COUNT;

        public override int GetItemViewType(int position)
        {
            ModifiableChoice item = GetItem(position);
            if (item.Choice.Separator)
            {
                return TYPE_SEPARATOR;
            }
            else if (_choicePromptType == GeckoSession.romptDelegateClassChoice.ChoiceTypeMenu)
            {
                return item.ModifiableSelected ? TYPE_MENU_CHECK : TYPE_MENU_ITEM;
            }
            else if (item.Choice.Items != null)
            {
                return TYPE_GROUP;
            }
            else if (_choicePromptType == GeckoSession.romptDelegateClassChoice.ChoiceTypeSingle)
            {
                return TYPE_SINGLE;
            }
            else if (_choicePromptType == GeckoSession.romptDelegateClassChoice.ChoiceTypeMultiple)
            {
                return TYPE_MULTIPLE;
            }
            else
            {
                throw new UnsupportedOperationException();
            }
        }

        public override bool IsEnabled(int position)
        {
            ModifiableChoice item = GetItem(position);
            return !item.Choice.Separator && !item.Choice.Disabled &&
                ((_choicePromptType != GeckoSession.romptDelegateClassChoice.ChoiceTypeSingle &&
                    _choicePromptType != GeckoSession.romptDelegateClassChoice.ChoiceTypeMultiple) ||
                    item.Choice.Items == null);
        }

        public override Android.Views.View GetView(int position, Android.Views.View view, ViewGroup parent)
        {
            var context = BlazorWebViewService.GetCurrentActivity();

            int itemType = GetItemViewType(position);
            int layoutId;
            if (itemType == TYPE_SEPARATOR)
            {
                if (mSeparator == null)
                {
                    mSeparator = new Android.Views.View(context);
                    mSeparator.LayoutParameters = new Android.Widget.ListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, 2, itemType);
                    TypedArray attr = context.ObtainStyledAttributes(
                            new int[] { Android.Resource.Attribute.ListDivider });
                    mSeparator.SetBackgroundResource(attr.GetResourceId(0, 0));
                    attr.Recycle();
                }
                return mSeparator;
            }
            else if (itemType == TYPE_MENU_ITEM)
            {
                layoutId = Android.Resource.Layout.SimpleListItem1;
            }
            else if (itemType == TYPE_MENU_CHECK)
            {
                layoutId = Android.Resource.Layout.SimpleListItemChecked;
            }
            else if (itemType == TYPE_GROUP)
            {
                layoutId = Android.Resource.Layout.PreferenceCategory;
            }
            else if (itemType == TYPE_SINGLE)
            {
                layoutId = Android.Resource.Layout.SimpleListItemSingleChoice;
            }
            else if (itemType == TYPE_MULTIPLE)
            {
                layoutId = Android.Resource.Layout.SimpleListItemMultipleChoice;
            }
            else
            {
                throw new UnsupportedOperationException();
            }

            if (view == null)
            {
                if (mInflater == null)
                {
                    mInflater = LayoutInflater.From(_builder.Context);
                }
                view = mInflater.Inflate(layoutId, parent, false);
            }

            ModifiableChoice item = GetItem(position);
            TextView text = (TextView)view;
            text.Enabled = !item.Choice.Disabled;
            text.Text = item.ModifiableLabel;
            if (view is CheckedTextView) {
                bool selected = item.ModifiableSelected;
                if (itemType == TYPE_MULTIPLE)
                {
                    _list.SetItemChecked(position, selected);
                }
                else
                {
                    ((CheckedTextView)view).Checked = selected;
                }
            }
            return view;
        }
    }

    public class BlazorGeckoViewPromptDelegate : Java.Lang.Object, GeckoSession.PromptDelegateClass
    {
        private static readonly Int32 REQUEST_CAMERA_TEST = 8;
        private static readonly Int32 SELECT_FILE = 9;

        private GeckoViewRenderer _renderer;

        public BlazorGeckoViewPromptDelegate(GeckoViewRenderer renderer)
        {
            _renderer = renderer;
        }

        private class PromptCompleteDismissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
        {
            System.Action _action;

            public PromptCompleteDismissListener(System.Action action)
            {
                _action = action;
            }

            public void OnDismiss(IDialogInterface dialog)
            {
                _action?.Invoke();
            }
        }

        private AlertDialog CreateStandardDialog(AlertDialog.Builder builder,
                                         GeckoSession.PromptDelegateClassChoiceCallback prompt)
        {
            AlertDialog dialog = builder.Create();
            dialog.SetOnDismissListener(new PromptCompleteDismissListener(() =>
            {
                //TODO: IsComplete not exposed through reflection of GeckoView Bindings
                //Need to check if this is mandatory. If yes we need to update the GeckoView library

                //if (!prompt.isComplete())
                //{
                //    response.complete(prompt.dismiss());
                //}

                prompt.Dismiss();
                dialog.Dispose();
            }));
            return dialog;
        }

        private int GetViewPadding(Android.App.AlertDialog.Builder builder)
        {
            TypedArray attr = builder.Context.ObtainStyledAttributes(
                    new int[] { Android.Resource.Attribute.ListPreferredItemPaddingLeft });
            int padding = attr.GetDimensionPixelSize(0, 1);
            attr.Recycle();
            return padding;
        }

        private Android.Widget.LinearLayout AddStandardLayout(Android.App.AlertDialog.Builder builder,
                                           string title, string msg)
        {
            Android.Widget.ScrollView scrollView = new Android.Widget.ScrollView(builder.Context);
            Android.Widget.LinearLayout container = new LinearLayout(builder.Context);
            int horizontalPadding = GetViewPadding(builder);
            int verticalPadding = string.IsNullOrEmpty(msg) ? horizontalPadding : 0;
            container.Orientation = Android.Widget.Orientation.Vertical;
            container.SetPadding(
                /* left */ horizontalPadding,
                /* top */ verticalPadding,
                /* right */ horizontalPadding,
                /* bottom */ verticalPadding);
            scrollView.AddView(container);
            builder.SetTitle(title)
                    .SetMessage(msg)
                    .SetView(scrollView);
            return container;
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

        private void AddChoiceItems(int type, ModifiableChoiceArrayAdapter<ModifiableChoice> list,
                            GeckoSession.romptDelegateClassChoice[] items, string indent)
        {
            if (type == GeckoSession.romptDelegateClassChoice.ChoiceTypeMenu)
            {
                foreach (GeckoSession.romptDelegateClassChoice item in items)
                {
                    list.Add(new ModifiableChoice(item));
                }
                return;
            }

            foreach (GeckoSession.romptDelegateClassChoice item in items)
            {
                ModifiableChoice modItem = new ModifiableChoice(item);

                GeckoSession.romptDelegateClassChoice[] children = item.Items?.ToArray();

                if (indent != null && children == null)
                {
                    modItem.ModifiableLabel = indent + modItem.ModifiableLabel;
                }
                list.Add(modItem);

                if (children != null)
                {
                    string newIndent;
                    if (type == GeckoSession.romptDelegateClassChoice.ChoiceTypeSingle || type == GeckoSession.romptDelegateClassChoice.ChoiceTypeMultiple)
                    {
                        newIndent = (indent != null) ? indent + '\t' : "\t";
                    }
                    else
                    {
                        newIndent = null;
                    }
                    AddChoiceItems(type, list, children, newIndent);
                }
            }
        }

        public virtual void OnChoicePrompt(GeckoSession session, string title, string msg, int type, GeckoSession.romptDelegateClassChoice[] choices, GeckoSession.PromptDelegateClassChoiceCallback prompt)
        {
            //This code is highly inspired of https://github.com/mozilla-mobile/focus-android/blob/f5b22ff78fca22765b8b873b4f70693943ae559a/app/src/main/java/org/mozilla/focus/gecko/GeckoViewPrompt.java#L265

            var currentActivity = BlazorWebViewService.GetCurrentActivity();

            if (currentActivity == null)
            {
                prompt.Dismiss();
                return;
            }

            var builder = new Android.App.AlertDialog.Builder(currentActivity);
            AddStandardLayout(builder, title, msg);

            Android.Widget.ListView list = new Android.Widget.ListView(builder.Context);
            if (type == GeckoSession.romptDelegateClassChoice.ChoiceTypeMultiple)
            {
                list.ChoiceMode = ChoiceMode.Multiple;
            }

            ModifiableChoiceArrayAdapter<ModifiableChoice> adapter = new ModifiableChoiceArrayAdapter<ModifiableChoice>(builder.Context, Android.Resource.Layout.SimpleListItem1, type, builder, list);
            AddChoiceItems(type, adapter, choices, /* indent */ null);

            list.SetAdapter(adapter);
            builder.SetView(list);

            AlertDialog dialog;
            if (type == GeckoSession.romptDelegateClassChoice.ChoiceTypeSingle || type == GeckoSession.romptDelegateClassChoice.ChoiceTypeMenu)
            {
                dialog = CreateStandardDialog(builder, prompt);

                list.ItemClick += (sender, e) =>
                {
                    ModifiableChoice item = adapter.GetItem(e.Position);
                    if (type == GeckoSession.romptDelegateClassChoice.ChoiceTypeMenu)
                    {
                        GeckoSession.romptDelegateClassChoice[] children = item.Choice.Items?.ToArray();
                        if (children != null)
                        {
                            // Show sub-menu.
                            dialog.SetOnDismissListener(null);
                            dialog.Dismiss();
                            OnChoicePrompt(session, item.ModifiableLabel, /* msg */ null,
                                    type, children, prompt);
                            return;
                        }
                    }
                    prompt.Confirm(item.Choice);
                    dialog.Dismiss();
                };
            }
            else if (type == GeckoSession.romptDelegateClassChoice.ChoiceTypeMultiple)
            {
                list.ItemClick += (sender, e) =>
                {
                    ModifiableChoice item = adapter.GetItem(e.Position);
                    item.ModifiableSelected = ((CheckedTextView)e.View).Checked;
                };
                builder
                    .SetNegativeButton(Android.Resource.String.Cancel, /* listener */ (IDialogInterfaceOnClickListener)null)
                    .SetPositiveButton(Android.Resource.String.Ok, (sender, e) =>
                        {
                            int len = adapter.Count;
                            List<string> items = new List<string>(len);
                            for (int i = 0; i < len; i++)
                            {
                                ModifiableChoice item = adapter.GetItem(i);
                                if (item.ModifiableSelected)
                                {
                                    items.Add(item.Choice.Id);
                                }
                            }
                            prompt.Confirm(items.ToArray());
                        });
                dialog = CreateStandardDialog(builder, prompt);
            }
            else
            {
                throw new UnsupportedOperationException();
            }
            dialog.Show();

            //TODO: Don't forget to Dispose dialogbuilder
        }

        public virtual void OnColorPrompt(GeckoSession session, string title, string value, GeckoSession.PromptDelegateClassTextCallback callback)
        {
        }

        public virtual void OnDateTimePrompt(GeckoSession session, string title, int type, string value, string min, string max, GeckoSession.PromptDelegateClassTextCallback callback)
        {
        }

        internal class IntentMetadata
        {
            public readonly Intent Intent;

            public readonly string[] RequiredPermissions;

            internal IntentMetadata(Intent intent, string[] requiredPermissions)
            {
                Intent = intent;
                RequiredPermissions = requiredPermissions;
            }
        }

        private void GetContentCompatibleIntents(int type, out List<IntentMetadata> intentList, out List<ResolveInfo> info)
        {
            Intent camIntent = new Intent("android.media.action.IMAGE_CAPTURE");
            Intent getActionIntents = new Intent(Intent.ActionGetContent);
            getActionIntents.SetType("*/*");
            getActionIntents.AddCategory(Intent.CategoryOpenable);

            //We must manage multiple file selection too
            if (type == FilePromptType.MULTIPLE)
            {
                getActionIntents.PutExtra(Intent.ExtraAllowMultiple, true);
            }

            // look for available intents
            info = new List<ResolveInfo>();
            intentList = new List<IntentMetadata>();
            PackageManager packageManager = BlazorWebViewService.GetCurrentActivity().PackageManager;
            List<ResolveInfo> listCam = packageManager.QueryIntentActivities(camIntent, 0).ToList();

            //Required permissions for camera intents
            string[] permissionsForCamera = new string[] {
                Android.Manifest.Permission.WriteExternalStorage,
                Android.Manifest.Permission.Camera
            };

            //Required permissions for read content intents
            string[] permissionsForGetContent = new string[] {
                Android.Manifest.Permission.ReadExternalStorage
            };

            foreach (ResolveInfo res in listCam)
            {
                Intent finalIntent = new Intent(camIntent);
                finalIntent.SetComponent(new ComponentName(res.ActivityInfo.PackageName, res.ActivityInfo.Name));
                intentList.Add(new IntentMetadata(finalIntent, permissionsForCamera));
                info.Add(res);
            }

            List<ResolveInfo> listGall = packageManager.QueryIntentActivities(getActionIntents, 0).ToList();
            foreach (ResolveInfo res in listGall)
            {
                Intent finalIntent = new Intent(getActionIntents);
                finalIntent.SetComponent(new ComponentName(res.ActivityInfo.PackageName, res.ActivityInfo.Name));
                intentList.Add(new IntentMetadata(finalIntent, permissionsForGetContent));
                info.Add(res);
            }
        }

        public class ResolveInfoAdapter<T> : ArrayAdapter<T> where T : ResolveInfo
        {
            private IList<T> ActivitiesInfo = new List<T>();

            private void SetInfos(IList<T> objects)
            {
                ActivitiesInfo = objects;
            }

            public ResolveInfoAdapter(Context context, int textViewResourceId, IList<T> objects) : base(context, textViewResourceId, objects)
            {
                SetInfos(objects);
            }

            public ResolveInfoAdapter(Context context, int textViewResourceId, T[] objects) : base(context, textViewResourceId, objects)
            {
                SetInfos(objects);
            }

            public ResolveInfoAdapter(Context context, int resource, int textViewResourceId, IList<T> objects) : base(context, resource, textViewResourceId, objects)
            {
                SetInfos(objects);
            }

            public ResolveInfoAdapter(Context context, int resource, int textViewResourceId, T[] objects) : base(context, resource, textViewResourceId, objects)
            {
                SetInfos(objects);
            }

            public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
            {
                Android.Views.View view = base.GetView(position, convertView, parent);
                ResolveInfo res = ActivitiesInfo.ElementAt(position);
                ImageView image = (ImageView)view.FindViewById(
                    BlazorWebViewService.GetResourceId("intent_listview_icon", AndroidResourceType.Identifier)
                );
                image.SetImageDrawable(res.LoadIcon(BlazorWebViewService.GetCurrentActivity().PackageManager));
                TextView textview = (TextView)view.FindViewById(
                    BlazorWebViewService.GetResourceId("intent_listview_title", AndroidResourceType.Identifier)
                    );
                textview.Text = res.LoadLabel(BlazorWebViewService.GetCurrentActivity().PackageManager);
                return view;
            }
        }

        private ResolveInfoAdapter<ResolveInfo> BuilderAdapter(Context context, List<ResolveInfo> activitiesInfo)
        {
            return new ResolveInfoAdapter<ResolveInfo>(context, Resource.Layout.intent_listview_row,
                //Resource.Id.intent_listview_title,
                BlazorWebViewService.GetResourceId("intent_listview_title", AndroidResourceType.Identifier),
                activitiesInfo);
        }

        private Android.Net.Uri GetFileForBrowser(Android.Net.Uri dataFromActivityResult)
        {
            try
            {
                string TempfileName = Guid.NewGuid().ToString("N") + ".file";

                string outputFolder = FileCachingHelper.GetFileCacheFolder();
                if (!System.IO.Directory.Exists(outputFolder))
                {
                    System.IO.Directory.CreateDirectory(outputFolder);
                }

                var backingFile = System.IO.Path.Combine(outputFolder, TempfileName);
                using (var output = System.IO.File.Create(backingFile))
                {
                    using (System.IO.Stream inputStream = BlazorWebViewService.GetCurrentActivity().ContentResolver.OpenInputStream(dataFromActivityResult))
                    {
                        inputStream.CopyTo(output);
                    }
                }

                return Android.Net.Uri.Parse("file://" + backingFile);
            }
            catch (System.Exception e)
            {
                ConsoleHelper.WriteException(e);
                throw;
            }
        }

        public virtual void OnFilePrompt(GeckoSession session, string title, int type, string[] mimeTypes, GeckoSession.PromptDelegateClassFileCallback callback)
        {
            var currentActivity = BlazorWebViewService.GetCurrentActivity();

            Action showActionsDialog = null;

            AlertDialog _futureDialog = null; //Workaround dismiss method not available on AlertDialog.Builder before Show();

            bool shouldDismiss = true;
            var dialogBuilder = new Android.App.AlertDialog.Builder(_renderer.Context);
            BlazorFileDialogDismissListener onDismissEvent = new BlazorFileDialogDismissListener();

            onDismissEvent.SetDismissHandler(() =>
            {
                if (shouldDismiss)
                {
                    callback.Dismiss();
                }

                dialogBuilder.Dispose();
            });

            dialogBuilder.SetOnDismissListener(onDismissEvent);

            GetContentCompatibleIntents(type, out List<IntentMetadata> intentList, out List<ResolveInfo> activitiesInfo);
            dialogBuilder.SetAdapter(BuilderAdapter(currentActivity, activitiesInfo),
                (sender, e) =>
                {
                    //On Intent click

                    shouldDismiss = false;
                    IntentMetadata selectedIntent = intentList.ElementAt(e.Which);

                    //Check for Android permissions for intents
                    RequestPermissionHelper.CheckForPermission(selectedIntent.RequiredPermissions, () =>
                    {
                        //On Intent permission availables

                        //We must manage multiple file selection too

                        //TODO: Even by adding this intent
                        //this seem to not working yet. Not sure if possible through launching external intents
                        if (type == FilePromptType.MULTIPLE)
                        {
                            selectedIntent.Intent.PutExtra(Intent.ExtraAllowMultiple, true);
                        }

                        currentActivity.StartActivityForResult(selectedIntent.Intent, 1, (int requestCode, Result resultCode, Intent data) =>
                        {
                            if (resultCode != Result.Ok)
                            {
                                //Do nothing as the user is still now on the Intent dialog box chooser

                                //As on Permission not granted, see comment below =>
                                //We should reset this value to true if denied, as the user may don't click on Cancel but use a back button instead
                                //As the back button will call the Dialog dismiss internally, we should emulate the cancel behavior (see SetNeutralButton code)
                                shouldDismiss = true;
                            }
                            else
                            {
                                try
                                {
                                    callback.Confirm(currentActivity, GetFileForBrowser(data.Data));
                                }
                                catch (System.Exception e)
                                {
                                    ConsoleHelper.WriteException(e);
                                    callback.Dismiss();
                                }

                                _futureDialog.Dismiss();
                            }
                        });

                    }, () =>
                    {
                        //On Intent permission unavailable

                        //We should reset this value to true if denied, as the user may don't click on Cancel but use a back button instead
                        //As the back button will call the Dialog dismiss internally, we should emulate the cancel behavior (see SetNeutralButton code)
                        shouldDismiss = true;
                    });
                });

                
            dialogBuilder.SetNeutralButton(currentActivity.Resources.GetString(Android.Resource.String.Cancel),
                (sender, e) =>
                {
                    shouldDismiss = true;
                    _futureDialog.Dismiss();
                });
            shouldDismiss = true;
            _futureDialog = dialogBuilder.Show();

            dialogBuilder.Show();
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