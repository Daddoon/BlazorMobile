using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using BlazorMobile.Components;
using BlazorMobile.Droid.Interop;
using BlazorMobile.Droid.Renderer;
using BlazorMobile.Interop;
using BlazorMobile.Services;
using System;
using System.IO;
using Xamarin.Forms;

namespace BlazorMobile.Droid.Services
{
    internal class AndroidBug5497WorkaroundForXamarinAndroid
    {

        // For more information, see https://code.google.com/p/android/issues/detail?id=5497
        // To use this class, simply invoke assistActivity() on an Activity that already has its content view set.

        // CREDIT TO Joseph Johnson (http://stackoverflow.com/users/341631/joseph-johnson) for publishing the original Android solution on stackoverflow.com

        public static void assistActivity(Activity activity)
        {
            new AndroidBug5497WorkaroundForXamarinAndroid(activity);
        }

        private Android.Views.View mChildOfContent;
        private int usableHeightPrevious;
        private FrameLayout.LayoutParams frameLayoutParams;

        private AndroidBug5497WorkaroundForXamarinAndroid(Activity activity)
        {
            FrameLayout content = (FrameLayout)activity.FindViewById(Android.Resource.Id.Content);
            mChildOfContent = content.GetChildAt(0);
            ViewTreeObserver vto = mChildOfContent.ViewTreeObserver;
            vto.GlobalLayout += (object sender, EventArgs e) => {
                possiblyResizeChildOfContent();
            };
            frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent.LayoutParameters;
        }

        private void possiblyResizeChildOfContent()
        {
            int usableHeightNow = computeUsableHeight();
            if (usableHeightNow != usableHeightPrevious)
            {
                int usableHeightSansKeyboard = mChildOfContent.RootView.Height;
                int heightDifference = usableHeightSansKeyboard - usableHeightNow;

                frameLayoutParams.Height = usableHeightSansKeyboard - heightDifference;

                mChildOfContent.RequestLayout();
                usableHeightPrevious = usableHeightNow;
            }
        }

        private int computeUsableHeight()
        {
            Rect r = new Rect();
            mChildOfContent.GetWindowVisibleDisplayFrame(r);
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                return (r.Bottom - r.Top);
            }
            return r.Bottom;
        }
    }

    [Android.Runtime.Preserve(AllMembers = true)]
    public static class BlazorWebViewService
    {
        private static Activity _activity = null;

        internal static Activity GetCurrentActivity()
        {
            return _activity;
        }

        private static void InitComponent(Android.App.Activity activity)
        {
            _activity = activity;

            activity.Window.SetSoftInputMode(SoftInput.AdjustResize | SoftInput.StateAlwaysHidden);

            //AndroidBug5497WorkaroundForXamarinAndroid.assistActivity(activity);

            DependencyService.Register<IWebViewService, WebViewService>();
            DependencyService.Register<IApplicationStoreService, ApplicationStoreService>();

            //Instanciate GeckoView type in BlazorMobile assemnly for Android
            BlazorWebViewFactory.SetInternalBlazorGeckoViewType(typeof(BlazorGeckoView));

            BlazorGeckoViewRenderer.Init(activity);
        }

        /// <summary>
        /// Init the WebApplicationFactory with the given app stream resolver.
        /// Shorthand for <see cref="WebApplicationFactory.RegisterAppStreamResolver" />
        /// </summary>
        /// <param name="appStreamResolver"></param>
        public static void Init(Android.App.Activity activity, Func<Stream> appStreamResolver)
        {
            InitComponent(activity);
            WebApplicationFactory.Init(appStreamResolver);
        }

        public static void Init(Android.App.Activity activity)
        {
            InitComponent(activity);
            WebApplicationFactory.Init();
        }
    }
}
