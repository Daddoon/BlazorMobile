using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.Res;
using BlazorMobile.Droid;
using BlazorMobile.Droid.Services;

namespace BlazorMobile.Template.Droid
{
    [Activity(Label = "BlazorMobile.Template", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static AssetManager assetManager = null;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            assetManager = this.Assets;

            global::Xamarin.Forms.Forms.Init(this, bundle);
            BlazorWebViewService.Init(this);

            LoadApplication(new App());
        }
    }
}

