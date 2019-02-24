# Blazor.Xamarin
Create Hybrid-Apps for iOS, Android, UWP with Blazor and Xamarin !
**Blazor.Xamarin** is a Nuget package for embedding Blazor application as standalone mobile application, hosted in Xamarin.

## Platform requirements

- **UWP:** Windows 10 Fall Creators Update (10.0 ; Build 16299) or greater
- **Android:** Android 5.0 - Lollipop (API 21) or greater
- **iOS:** iOS 12 or greater
- **Blazor:** 0.8.0

As Blazor is evolving very fast, this current plugin implementations may become obsolete on some future versions.
**The current version has been developed and tested on Blazor 0.8.0-preview-19104-04**

### Pro

- You can now develop your application for Web, iOS, Android, UWP, with nearly the same code base !
- Interop support between the Blazor Hybrid apps and Xamarin native apps
- All in C# !

### Cons

- No direct access to FileSystem from the Blazor app. You should use the interop communication layer and Xamarin capabilities in order to emulate theses functionnalities.
- Lack of debug functionnalities, like the official Blazor WASM-mode scenario
- Harder to debug: As you don't have a direct access to the browser embbeding your Blazor application, you have to read the platform specific debug output in order to read console messages or other exceptions.

**NOTE:** You may try to consider debugging from the **Microsoft SignalR** Blazor (Server Mode) implementation or calling an external logging API in order to follow your exceptions.

**Also keep in mind that you can read platform specific outputs from Visual Studio output window when in debug mode !** 

### Side note

- The current tooling on Blazor 0.8.0 does **NOT** yet implement a full **mono-wasm-aot toolchain**. This mean that you Blazor app will still use the old JIT interpretation mode. Your app can be 10x slower than the native .NET...
- **...On the other side**, this also mean that when the Blazor mono toolchain will be AOT compatible and available, your mobile Blazor app will also gain performance from that !
- There is also an experimental Blazor support from Microsoft Team, on **UWP** and **MacOS**, that was made with **Electron.NET** .
This is not affiliated with this project, and not officialy supported at the moment. But we may see some cool platform support in the future considering this.

## Summary

- [Installing Blazor.Xamarin from scratch](https://github.com/Daddoon/Blazor.Xamarin/#installing-blazorxamarin-from-scratch)
- [Communication between Blazor/Xamarin.Forms](https://github.com/Daddoon/Blazor.Xamarin/#communication-between-blazorxamarinforms)
- [Detecting Runtime Platform](https://github.com/Daddoon/Blazor.Xamarin/blob/master/README.md#detecting-runtime-platform)

### Easy installation

You may just copy and use all **Daddoon.Blazor.Xam.InteropApp projects**, located in the **Test** solution folder of this project.

I advise you to also read the [installation guide](https://github.com/Daddoon/Blazor.Xamarin/#installing-blazorxamarin-from-scratch) to know how the initial setup works.

For communication between your **Blazor app and Native Xamarin app** [COMMUNICATION BETWEEN BLAZOR/XAMARIN.FORMS section](https://github.com/Daddoon/Blazor.Xamarin#communication-between-blazorxamarinforms).


## Installing Blazor.Xamarin from scratch

### 1. Create your Xamarin.Forms application project in Visual Studio

The ideal scenario, as the given templates in this repository, is to create a Cross-plateform Xamarin project template.
You then should have your solution this type of configuration:

- YourApp (.netstandard2.0)
- YourApp.Droid (MonoDroid)
- YourApp.iOS (Xamarin.iOS)
- YourApp.UWP (UWP)

**YourApp** project will be used as the Blazor app container, it's not mandatory but highly recommended.

**NOTE:** It is also advised to create an additional shared project (.netstandard2.0) with no Xamarin.Forms reference, in order to use it to share interface contracts between Blazor and Xamarin domains for interop communication. We will assume that this shared project will be called **YourApp.Shared** !

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
- There could be some issue with old browsers that doesn't support some ECMA script standards. You may want to fill the gap in your Blazor project by adding some polyfills. If so, take a look at my [Blazor.Polyfill](https://github.com/Daddoon/Blazor.Polyfill) repository. This requirement is less mandatory here are i highly recommand to use a modern mobile Browser for WASM performance with Blazor.

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

Also, you must update your **Info.plist** file to allow localhost requests inside your app by adding the **NSAppTransportSecurity** property. Your file should look like this:
```xml
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

Also, you must update your **Package.appxmanifest** file to allow localhost requests inside your app, by adding the **ApplicationContentUriRules** property. You must add the following property in the following hierarchy:

```xml
<Package>
	<Applications>
		<Application Id="App">
			 <uap:ApplicationContentUriRules>
          		 	<uap:Rule Type="include" Match="http://localhost:8888/" WindowsRuntimeAccess="all" />
      			 </uap:ApplicationContentUriRules>
		</Application>
	</Applications>
</Package>
```

You may consider using an other port than **8888** in this rule, but you must be sure that your port rule here match with your port configuration in you app code (see below)


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

            WebApplicationFactory.SetHttpPort(8888);

            //Regiser Blazor app resolver
            //CUSTOMIZE HERE YOUR OWN CODE LOGIC IF NEEDED !!
            WebApplicationFactory.RegisterAppStreamResolver(() =>
            {
                //Get current class Assembly object
                var assembly = typeof(App).Assembly;

                //Name of our current Blazor package in this project, stored as a Embedded Resource
                string BlazorPackageFolder = "Mobile.package.app.zip";

                string appPackage = $"{assembly.GetName().Name}.{BlazorPackageFolder}";

                return assembly.GetManifestResourceStream(appPackage);
            });

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

As you can see you must provide yourself your own logic to retrieve the Blazor app ZIP file with the method **WebApplicationFactory.RegisterAppStreamResolver**.
The method is waiting for a delegate method that will return the ZIP stream data of your Blazor app. You can use the same code logic, and juste modify the file/paths used, in accordance to your project.

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

In order to communicate from Blazor to Xamarin you need to do some few steps, as JIT is disabled on AOT environment like Blazor.
Here is a simple example to Display a Xamarin.Forms alert from Blazor.

**In your YourApp.Shared project**, create an interface in an Interfaces folder, and add the ProxyInterface attribute on it. Assuming a **IXamarinBridge** interface class.

Your file should look like this:

```csharp
using Daddoon.Blazor.Xam.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YourApp.Shared.Interfaces
{
    [ProxyInterface]
    public interface IXamarinBridge
    {
        Task<List<string>> DisplayAlert(string title, string msg, string cancel);
    }
}

```

**In your YourApp project**, implement the concrete implementation, also referenced as a DependencyService. On **UWP**, you may have to reference your service at startup with **Xamarin.Forms.DependencyService.Register<T>** as stated on this [DependencyService documentation](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/dependency-service/introduction#universal-windows-platform-net-native-compilation)
	
Your implementation may look like this. Here a some idiotic example:

```csharp
using YourApp.Shared.Interfaces;
using YourApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(XamarinBridge))]
namespace YourApp.Services
{
    public class XamarinBridge : IXamarinBridge
    {
        public Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            App.Current.MainPage.DisplayAlert(title, msg, cancel);

            List<string> result = new List<string>()
            {
                "Lorem",
                "Ipsum",
                "Dolorem",
            };

            return Task.FromResult(result);
        }
    }
}
```

**In your Blazor project**, implement the proxy class implementation, assuming the **BlazorApp** namespace is your Blazor application default namespace. For our example it look like this:

```csharp
using Daddoon.Blazor.Xam.Common.Services;
using YourApp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Services
{
    public class XamarinBridgeProxy : IXamarinBridge
    {
        public Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            return MethodDispatcher.CallMethodAsync<List<string>>(MethodBase.GetCurrentMethod(), title, msg, cancel);
        }
    }
}
```

**The key is the MethodDispatcher** class, that will prepare every callback for you, but because of the lack of JIT, you have to give yourself some parameters. Take a look at the different implementations of MethodDispatcher methods, in order to accord everything to your context, like if your using Task (Async calls) or not, if you expect a return value, generic types etc.

There is actually some syntactic sugar method calls in order to just mimic what you are expecting, by just recoying the same kind of signature, if using generic parameters etc. You may take a look at the [MethodDispatcher file](https://github.com/Daddoon/Blazor.Xamarin/blob/master/src/Daddoon.Blazor.Xamarin.Common/Services/MethodDispatcher.cs) if you want to see the available methods overload.

**Note that if you want that the caller and receiver is actually the same method signature on the 2 ends (Blazor and Xamarin), you can safely use MethodBase.GetCurrentMethod() everytime for the MethodInfo parameter**


## Test your interop with Xamarin in Blazor

Don't forget to add your Blazor implementation in the dependency services of your Blazor app, even if it's not mandatory of course.
Assuming in your **Program.cs** file of your **Blazor project**:

```csharp
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new BrowserServiceProvider(services =>
            {
                services.AddSingleton<IXamarinBridge, XamarinBridgeProxy>();
                // Add any custom services here
            });

            var br = new BrowserRenderer(serviceProvider);
            br.AddComponent<App>("app");
        }
    }
```

Then in one of your desired cshtml page (or .cs file btw), juste add
```csharp
@inject IXamarinBridge XamarinBridge
```

On top of your cshtml file, then call your method in your desired callback, like:

```csharp
var result = await XamarinBridge.DisplayAlert("MyTitle", "Blazor to Xamarin.Forms call works!", "Thanks!");
```

Note that our implementation in Xamarin does not wait for the user input validation, just change the code to your needs.

# Detecting Runtime Platform

In order to detect the current runtime environment of your Blazor app within Blazor, you must set the following in your **Program.cs** file of your Blazor project:

```csharp

//SOME CODE
br.AddComponent<App>("app");

BlazorWebViewService.Init(br, "blazorXamarin", () =>
{
   //Your code
});
```

Where **blazorXamarin** is a tag name available in your **index.html** like:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="initial-scale=1.0" />
    <title>Daddoon.Blazor.Xam.InteropBlazorApp</title>
    <base href="/" />
    <link href="css/bootstrap/bootstrap.min.css" rel="stylesheet" />
    <link href="css/site.css" rel="stylesheet" />
</head>
<body>
    <app>Loading...</app>
    <blazorXamarin></blazorXamarin>
    <script src="css/bootstrap/bootstrap-native.min.js"></script>
    <script type="text/javascript" src="js/blazor.polyfill.js"></script>
    <script type="blazor-boot"></script>
    <script></script>
</body>
</html>
```

...as it is necessary to inject a minimal javascript code in order to be able to check if we are on a pure browser context, or in an Hybrid app.

The last parameter of the Init method is an optional callback to notify when the initialization is finished.

You can then detect your current runtime platform at anytime by calling:

```csharp
Device.RuntimePlatform
```

In order to manage your application workflow and specific services/calls based on the underlying system.
**Device.RuntimePlatform** namespace is **Daddoon.Blazor.Xam.Common** and does mimic the result of **Xamarin.Forms.Device.RuntimePlatform** with some minor change, as **Browser** is returned in a pure web app in a browser, and **Unknown** is returned if an error occur or if the initialization is not yet made.


# DISCLAIMER

This project is not affiliated with the Blazor project.
