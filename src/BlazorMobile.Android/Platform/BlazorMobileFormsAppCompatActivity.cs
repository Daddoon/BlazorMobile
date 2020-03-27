
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views.Accessibility;
using BlazorMobile.Droid.Helper;
using System;

namespace BlazorMobile.Droid.Platform
{
    //This class is more for convenience as the app is focused on Blazor
    //In a standard way, we should have used some fragment or additional activities in order to catch the OnActivityResult method
    public class BlazorMobileFormsAppCompatActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private Action<int, Result, Intent> _onResult;
        private int _currentRequestCode = -1;

        internal void StartActivityForResult(Intent intent, int requestCode, Action<int, Result, Intent> onResult)
        {
            //This is not totally correct due to the fact that the given delegate can change and that theses call may be asynchronous.
            //But as we are mainly calling one Activity per action, this should never be a problem
            _onResult = onResult;
            _currentRequestCode = requestCode;
            StartActivityForResult(intent, requestCode);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            #if DEBUG
            FileCachingHelper.ClearCache();
            #else
            FileCachingHelper.ClearCache(TimeSpan.FromDays(1));
            #endif
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == _currentRequestCode)
            {
                _currentRequestCode = -1;
                if (_onResult != null)
                {
                    _onResult(requestCode, resultCode, data);
                    _onResult = null;
                }
            }
        }
    }
}