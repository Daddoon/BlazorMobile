using Android.Graphics;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Views.InputMethods;
using System;

namespace BlazorMobile.Droid.Helper
{
    /**
     * Created by Guillaume ZAHRA on 16.08.19. This is a C# adaptation of Mike Penz plugin Keyboardutil
     * Below inital credits
     * 
     * Created by mikepenz on 14.03.15.
     * This class implements a hack to change the layout padding on bottom if the keyboard is shown
     * to allow long lists with editTextViews
     * Basic idea for this solution found here: http://stackoverflow.com/a/9108219/325479
     */
    public class KeyboardUtil
    {
        private View decorView;
        private View contentView;
        private OnGlobalLayoutListener onGlobalLayoutListener;

        public KeyboardUtil(Activity act, View contentView)
        {
            this.decorView = act.Window.DecorView;
            this.contentView = contentView;

            onGlobalLayoutListener = new OnGlobalLayoutListener(this.decorView, this.contentView);

            //only required on newer android versions. it was working on API level 19
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                decorView.ViewTreeObserver.AddOnGlobalLayoutListener(onGlobalLayoutListener);
            }
        }

        public void Enable()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                decorView.ViewTreeObserver.AddOnGlobalLayoutListener(onGlobalLayoutListener);
            }
        }

        public void Disable()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                decorView.ViewTreeObserver.RemoveOnGlobalLayoutListener(onGlobalLayoutListener);
            }
        }


        public class OnGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
        {
            private View _decorView;
            private View _contentView;

            public OnGlobalLayoutListener(View decorView, View contentView)
            {
                _decorView = decorView;
                _contentView = contentView;
            }

            public void OnGlobalLayout()
            {
                Rect r = new Rect();
                //r will be populated with the coordinates of your view that area still visible.
                _decorView.GetWindowVisibleDisplayFrame(r);

                //get screen height and calculate the difference with the useable area from the r
                int height = _decorView.Context.Resources.DisplayMetrics.HeightPixels;
                int diff = height - r.Bottom;

                //if it could be a keyboard add the padding to the view
                if (diff != 0)
                {
                    // if the use-able screen height differs from the total screen height we assume that it shows a keyboard now
                    //check if the padding is 0 (if yes set the padding for the keyboard)
                    if (_contentView.PaddingBottom != diff)
                    {
                        //set the padding of the contentView for the keyboard
                        _contentView.SetPadding(0, 0, 0, diff);
                    }
                }
                else
                {
                    //check if the padding is != 0 (if yes reset the padding)
                    if (_contentView.PaddingBottom != 0)
                    {
                        //reset the padding of the contentView
                        _contentView.SetPadding(0, 0, 0, 0);
                    }
                }
            }
        }

        /**
         * Helper to hide the keyboard
         *
         * @param act
         */
        public static void HideKeyboard(Activity act)
        {
            if (act != null && act.CurrentFocus != null)
            {
                InputMethodManager inputMethodManager = (InputMethodManager)act.GetSystemService(Activity.InputMethodService);
                inputMethodManager.HideSoftInputFromWindow(act.CurrentFocus.WindowToken, 0);
            }
        }
    }
}