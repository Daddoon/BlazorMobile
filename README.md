# Blazor.Xamarin
A Nuget package for launching Blazor application as standalone application on Xamarin

# INSTALLATION

## 1. Create your Xamarin.Forms application project in Visual Studio

The ideal scenario, as the given templates in this repository, is to create a Cross-plateform Xamarin project template.
You then should have your solution this type of configuration:

- YourApp (.netstandard2.0)
- YourApp.Droid (MonoDroid)
- YourApp.iOS (Xamarin.iOS)
- YourApp.UWP (UWP)

**YourApp** project will be used as the Blazor app container, it's not mandatory but highly recommended.

**NOTE:** It is also advised to create an additional shared project (.netstandard2.0) with no Xamarin.Forms reference, in order to use it to share interface contracts between Blazor and Xamarin domains for interop communication.

Assuming this shared project called **YourApp.Shared** !

## 2. ZIP your Blazor app project ! Our plugin need to read a Blazor app zipped in an archive for maintenability convenience.

As you surely want to always have you Blazor app in sync in your mobile standalone app, you may want to automate your ZIP archive content.
The Blazor example template use this command at PostBuild event:

```
rm $(ProjectDir)\BuildTools\Mobile\bin\app.zip >nul 2>&1
$(ProjectDir)\BuildTools\7za.exe a $(ProjectDir)\BuildTools\Mobile\bin\app.zip $(ProjectDir)wwwroot\* -mx1 -tzip
$(ProjectDir)\BuildTools\7za.exe a $(ProjectDir)\BuildTools\Mobile\bin\app.zip $(ProjectDir)$(OutputPath)dist\* -mx1 -tzip
```

Of course adapt the path to your development environement. If use this method, notice to respect the current order, first **wwwroot**, then **dist**, as the dist folder contain also an index.html file, but processed by Blazor tooling: This is the right one to use, not the one you see available in your solution when coding.

## 3. Blazor changes for mobile

You may need to do some changes on your Blazor project, in order to render and work correctly under Xamarin and/or mobile web browsers.

**All:**
- There could be some issue with old browsers that doesn't support some ECMA script standards. You may want to fill the gap in your Blazor project by adding some polyfills. If so, take a look at my [Blazor.Polyfill](https://github.com/Daddoon/Blazor.Polyfill) repository.

**iOS:**

Add:

```html
<meta name="viewport" content="initial-scale=1.0" />
```

As child of your **head** tag, in your **index.html** file of your Blazor project, in order to render the web application on iOS with the native scaling;



## 4. Add your Blazor ZIP file as link in YourApp project

On YourApp project, add your generated ZIP from the Blazor project, as a "link" => Right click on the project => Add existing file => Browse to your file => Click on the little arrow => Then click on **Add as link**

## 5. Set your linked file as Embedded Resource

Do right click on your newly added as link file in YourApp project, and click **Properties**
Then check that the **Build Action property** is on **Embedded Resource**

## 6. Add Daddoon.Blazor.Xamarin NuGet package

Add **Daddoon.Blazor.Xamarin** NuGet package on the following projects:

- YourApp
- YourApp.Droid
- YourApp.iOS
- YourApp.UWP

Add **Daddoon.Blazor.Xamarin.Common** NuGet package on the following projects:

- YourApp.Shared
- Your Blazor project, but actually **YourApp.Shared may be sufficient**, as it will be referenced also on your Blazor project.

The packages are available on the nuget.org feed, but you can also download the file manually [in the release page](https://github.com/Daddoon/Blazor.Xamarin/releases)

## 7. Platform specific configuration

As there is often some strange behavior with IL stripping in Xamarin, you have to call an init method on each platform.

For **Android** you have to set the following in **MainActivity.cs**
```csharp
using Daddoon.Blazor.Xam.Droid.Services;
    
namespace YourApp.Droid
{
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            /* Some other code */
            global::Xamarin.Forms.Forms.Init(this, bundle);
            BlazorWebViewService.Init();
            /* Some other code */
        }
     }
}
```

For **iOS** you have to set the following in **AppDelegate.cs**
```csharp
using Daddoon.Blazor.Xam.iOS.Services;
    
namespace YourApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            /* Some other code */
            global::Xamarin.Forms.Forms.Init(this, bundle);
            BlazorWebViewService.Init();
            /* Some other code */
        }
     }
}
```

Also, you must update your **Info.plist** file to allow localhost requests inside your app. Your file should look like this:
```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
      /* OTHER ENTRIES */
     <key>NSAppTransportSecurity</key>
     <dict>
	 <key>NSAllowsArbitraryLoads</key>
	 <true/>
	 <key>NSExceptionDomains</key>
	 <dict>
	     <key>localhost</key>
	     <dict>
	         <key>NSExceptionAllowsInsecureHTTPLoads</key>
		 <true/>
		 <key>NSIncludesSubdomains</key>
		 <true/>
	     </dict>
	 </dict>
     </dict>
</dict>
</plist>
```

For **UWP** you have to set the following in **App.xaml.cs**
```csharp
using Daddoon.Blazor.Xam.UWP.Services;
    
namespace YourApp.UWP
{
    sealed partial class App : Application
    {
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            /* Some other code */
 	    Xamarin.Forms.Forms.Init(e);
            BlazorWebViewService.Init();
            /* Some other code */
        }
     }
}
```

## 8. Cross-reference YourApp.Shared

If you intend to do interop call from Blazor to Xamarin, you may now reference your shared project to Xamarin and Blazor projects.

- Add reference **YourApp.Shared** on **YourApp** project
- Add reference **YourApp.Shared** on **Your Blazor project**

## 9. Shared/Common project configuration

There are few, but still some lines to add to **YourApp** project

Open **App.xaml.cs** and make it look like this:

```csharp
using Daddoon.Blazor.Xam.Services;
using Xamarin.Forms;

namespace YourApp
{
    public partial class App : Application
    {
        public App ()
	{
	    InitializeComponent();

            //Regiser Blazor app resolver
            WebApplicationFactory.RegisterAppStreamResolver(BlazorAppResolver.GetAppStream);

	    MainPage = new MainPage();
	}

	protected override void OnStart ()
	{
            // Handle when your app starts
            WebApplicationFactory.StartWebServer();
	}

	protected override void OnSleep ()
	{
            // Handle when your app sleeps
            WebApplicationFactory.StopWebServer();
        }

	protected override void OnResume ()
	{
            WebApplicationFactory.StartWebServer();
        }
    }
}
```

Your IDE will complain that it don't find **BlazorAppResolver.GetAppStream** . It's totally fine, because you must provide your own delegate of **how do you get your Blazor app ZIP file**.

Here is a code snippet of our template, assuming your ZIP file is in a **Mobile\package\app.zip** directory hierarchy in **YourApp** project:

```csharp
using System.IO;

namespace YourApp.Resolver
{
    public static class BlazorAppResolver
    {
        private static string BlazorPackageFolder = "Mobile.package.app.zip";

        public static Stream GetAppStream()
        {
            var assembly = typeof(BlazorAppResolver).Assembly;

            string appPackage = $"{assembly.GetName().Name}.{BlazorPackageFolder}";

            return assembly.GetManifestResourceStream(appPackage);
        }
    }
}
```

Our library will manage when to get and dispose the stream, don't worry about that !

## 10. Add BlazorWebView component to your MainPage

Add BlazorWebView component to your MainPage.xaml, or actually any Xamarin.Forms page you would like to the Blazor app to launch.

Here is a example of how your **MainPage.xaml** and **MainPage.xaml.cs** could look like. Actually pretty idiotic configuration about the bounds of you WebView, we strongly advise you to update everything to your requirement. There will be more exhaustive example here in the future.

**MainPage.xaml**
```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:YourApp"
             x:Class="YourApp.MainPage">
    <ContentPage.Content>
        <StackLayout x:Name="content" HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand">
            
        </StackLayout>
    </ContentPage.Content>
</ContentPage>
```

**MainPage.xaml.cs**
```csharp
using Daddoon.Blazor.Xam.Components;
using Daddoon.Blazor.Xam.Services;
using Xamarin.Forms;

namespace YourApp
{
    public partial class MainPage : ContentPage
    {
	public MainPage()
	{
	    InitializeComponent();

            var url = new UrlWebViewSource
            {
                Url = WebApplicationFactory.GetBaseURL()
            };

            BlazorWebView webview = new BlazorWebView()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 1000,
                WidthRequest = 1000,
                Source = url
            };
            content.Children.Add(webview);
        }
    }
}
```

## 11. That's all !

You may now try to launch your app on your Device/Simulator, your Blazor app should start!

**NOTE:** There is some buggy Chrome (version 55.x) on Android 7.1 on the simulator that may crash the app (Out of Memory error within the Chrome activity). This behavior as not been seen on other and even older versions.

# COMMUNICATION BETWEEN BLAZOR/XAMARIN.FORMS

**TODO**: Not yet, but very soon!

# DISCLAIMER

This project is not affiliated with the Blazor project.
