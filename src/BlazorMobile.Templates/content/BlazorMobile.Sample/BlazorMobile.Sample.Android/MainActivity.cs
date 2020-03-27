using Android.App;
using Android.Content.PM;
using Android.OS;
using BlazorMobile.Droid.Services;
using BlazorMobile.Services;
using BlazorMobile.Sample.AppPackage;
using BlazorMobile.Droid.Platform;

namespace BlazorMobile.Sample.Droid
{
    [Activity(Label = "BlazorMobile.Sample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : BlazorMobileFormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            BlazorWebViewService.Init(this);

            //Register our Blazor app package
            WebApplicationFactory.RegisterAppStreamResolver(AppPackageHelper.ResolveAppPackageStream);

            LoadApplication(new App());
        }
    }
}

