# BlazorMobile
Create full C# driven hybrid-apps for iOS & Android!

**BlazorMobile** - formerly Blazor.Xamarin - is a Nuget package for embedding a Blazor application as a standalone mobile application, hosted in a Xamarin app.

## Platform requirements

- ~~**UWP:** Windows 10 Fall Creators Update (10.0 ; Build 16299) or greater~~ (Removed for this release)
- **Android:** Android 4.4 or greater
- **iOS:** iOS 12 or greater
- **Blazor:** 0.8.0

As Blazor is evolving very fast, this current plugin implementations may become obsolete on some future versions.


**The current version has been developed and tested on Blazor 0.8.0-preview-19104-04**

### Side note

- The current tooling on Blazor 0.8.0 does **NOT** yet implement a full **mono-wasm-aot toolchain**. This mean that you Blazor app will still use the old JIT interpretation mode. Your app can be 10x slower than the native .NET.
- This also mean that when the Blazor mono toolchain will be AOT compatible and available, your mobile Blazor app will also gain performance from that.
- This documentation may lack of some integration details, feel free to ask question in issues, if the sample code is not sufficient.

## Summary

- [Getting started from sample](https://github.com/Daddoon/BlazorMobile#getting-started-from-sample)
- [Installing BlazorMobile from scratch](https://github.com/Daddoon/BlazorMobile#installing-blazormobile-from-scratch)
- [Communication between Blazor/Xamarin.Forms](https://github.com/Daddoon/BlazorMobile#communication-between-blazorxamarinforms)
- [Detecting Runtime Platform](https://github.com/Daddoon/BlazorMobile#detecting-runtime-platform)

### Getting started from sample

The easiest way in order to start is to [download the sample projects](https://github.com/Daddoon/BlazorMobile/releases/download/0.8.0/BlazorMobile.Samples.zip). Unzip, and open the solution, and you are good to go.

If you want to install from scratch, read below.


### Installing BlazorMobile from scratch

#### 1. Create your Xamarin.Forms application project in Visual Studio

The ideal scenario, as the given templates in this repository, is to create a Cross-plateform Xamarin project template.
You then should have your solution this type of configuration:

- YourApp (.netstandard2.0)
- YourApp.Droid (MonoDroid)
- YourApp.iOS (Xamarin.iOS)

**YourApp** project will be used as the Blazor app container, it's not mandatory but highly recommended.

**NOTE:** It is also advised to create an additional shared project (.netstandard2.0) with no Xamarin.Forms reference, in order to use it to share interface contracts between Blazor and Xamarin domains for interop communication. We will assume that this shared project will be called **YourApp.Shared** !

#### 2. Zip your Blazor application project.

As you surely want to always have you Blazor app in sync in your mobile standalone app, you may want to automate your ZIP archive content.
The Blazor example template use this command at PostBuild event:

```
rm $(ProjectDir)\BuildTools\Mobile\bin\app.zip >nul 2>&1
$(ProjectDir)\BuildTools\7za.exe a $(ProjectDir)\BuildTools\Mobile\bin\app.zip $(ProjectDir)wwwroot\* -mx1 -tzip
$(ProjectDir)\BuildTools\7za.exe a $(ProjectDir)\BuildTools\Mobile\bin\app.zip $(ProjectDir)$(OutputPath)dist\* -mx1 -tzip
```

Of course adapt the path to your development environement. If you use this method, notice to respect the current order:
- First **wwwroot**
- Then **dist**, as the dist folder contain also an index.html file, but processed by Blazor tooling. This is the right one to use.

**Integrated project syncing may be included in the future, but for the moment, this is one of the easy way.**

#### 3. Blazor changes for mobile

You may need to do some changes on your Blazor project, in order to render and work correctly.

**Mobile application scaling behavior**

Add:

```html
<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
```

As child of your **head** tag, in your **index.html** file of your Blazor project.



#### 4. Add your Blazor Zip file as link in YourApp project

On YourApp project, add your generated ZIP from the Blazor project, as a "link" => Right click on the project => Add existing file => Browse to your file => Click on the little arrow => Then click on **Add as link**

#### 5. Set your linked file as Embedded Resource

Do right click on your newly added as link file in YourApp project, and click **Properties**
Then check that the **Build Action property** is on **Embedded Resource**

#### 6. Add BlazorMobile NuGet package

Add **BlazorMobile** NuGet package on the following projects:

- YourApp
- YourApp.Droid
- YourApp.iOS

Add **BlazorMobile.Common** NuGet package on the following projects:

- YourApp.Shared
- Your Blazor application

The packages are available on the nuget.org feed, but you can also download the file manually [in the release page](https://github.com/Daddoon/BlazorMobile/releases)

#### 7. Platform specific configuration

As there is often some strange behavior with IL stripping in Xamarin, you have to call an init method on each platform.

#### Android platform

For **Android** you have to set the following in **MainActivity.cs**
```csharp
using BlazorMobile.Droid.Services;
    
namespace YourApp.Droid
{
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            /* Some other code */
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            BlazorWebViewService.Init(this);
            /* Some other code */
        }
     }
}
```

#### iOS platform

For **iOS** you have to set the following in **AppDelegate.cs**
```csharp
using BlazorMobile.iOS.Services;
    
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


#### 8. Cross-reference YourApp.Shared

If you intend to do interop call from Blazor to Xamarin, you may now reference your shared project to Xamarin and Blazor projects.

- Add reference **YourApp.Shared** on **YourApp** project
- Add reference **YourApp.Shared** on **Your Blazor project**

**NOTE:** A simpler communication system, for broadcasting messages, may come in future release.

#### 9. Shared/Common project configuration

There are few, but still some lines to add to **YourApp** project

Open **App.xaml.cs** and make it look like this:

```csharp
using BlazorMobile.Services;
using Xamarin.Forms;

namespace YourApp
{
    public partial class App : Application
    {
        public App ()
	{
	    InitializeComponent();

#if DEBUG
            WebApplicationFactory.EnableDebugFeatures();
#endif
            //Change your application listening port
            WebApplicationFactory.SetHttpPort(8888);

            //Register Blazor application package resolver
            WebApplicationFactory.RegisterAppStreamResolver(() =>
            {
                //This app assembly
                var assembly = typeof(App).Assembly;

                //Name of our current Blazor package in this project, stored as an "Embedded Resource"
                //The file is resolved through AssemblyName.NamespaceFolder.app.zip
                return assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Package.app.zip");
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

You must provide yourself your own logic to retrieve the Blazor app ZIP file with the method **WebApplicationFactory.RegisterAppStreamResolver**.
The method is waiting for a delegate method that will return the ZIP stream data of your Blazor app. You can use the same code logic, and juste modify the file/paths used, in accordance to your project.

#### 10. Add BlazorWebView component to your MainPage

Add BlazorWebView component to your MainPage.xaml, or actually any Xamarin.Forms page you would like to the Blazor app to launch.

Here is a example of how your **MainPage.xaml** and **MainPage.xaml.cs** could look like. Actually pretty idiotic configuration about the bounds of you WebView, we strongly advise you to update everything to your requirement. There will be more exhaustive example here in the future.

**MainPage.xaml**
```xml
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
using BlazorMobile.Components;
using BlazorMobile.Services;
using Xamarin.Forms;

namespace YourApp
{
    public partial class MainPage : ContentPage
    {
	public MainPage()
	{
            InitializeComponent();

            //Blazor WebView agnostic contoller logic
            IBlazorWebView webview = BlazorWebViewFactory.Create();

            //WebView rendering customization on page
            View webviewView = webview.GetView();
            webviewView.VerticalOptions = LayoutOptions.FillAndExpand;
            webviewView.HorizontalOptions = LayoutOptions.FillAndExpand;

            webview.LaunchBlazorApp();

            content.Children.Add(webviewView);
        }
    }
}
```

#### 11. Add blazorXamarin tag to your index.html file

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>BlazorMobile.BlazorApp</title>
    <base href="/" />
    <link href="css/bootstrap/bootstrap.min.css" rel="stylesheet" />
    <link href="css/site.css" rel="stylesheet" />
</head>
<body>
    <app>Loading...</app>
    <script type="text/javascript" src="js/blazor.polyfill.js"></script>
    <script src="_framework/components.webassembly.js"></script>
    <blazorXamarin></blazorXamarin>
</body> 
</html>
```

#### 12. Device test

You may now try to launch your app on your Device/Simulator, your Blazor app should start!

### Communication between Blazor/Xamarin.Forms

In order to communicate from Blazor to Xamarin you need to do some few steps, as JIT is disabled on AOT environment like Blazor.
Here is a simple example to Display a Xamarin.Forms alert from Blazor.

**In your YourApp.Shared project**, create an interface in an Interfaces folder, and add the ProxyInterface attribute on it. Assuming a **IXamarinBridge** interface class.

Your file should look like this:

```csharp
using BlazorMobile.Common.Attributes;
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

**In your YourApp project**, implement the concrete implementation, also referenced as a DependencyService.
	
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
using BlazorMobile.Common.Services;
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

There is actually some syntactic sugar method calls in order to just mimic what you are expecting, by just recoying the same kind of signature, if using generic parameters etc. You may take a look at the [MethodDispatcher file](https://github.com/Daddoon/BlazorMobile/blob/master/src/BlazorMobile.Common/Services/MethodDispatcher.cs) if you want to see the available methods overload.

**Note that if you want that the caller and receiver is actually the same method signature on the 2 ends (Blazor and Xamarin), you can safely use MethodBase.GetCurrentMethod() everytime for the MethodInfo parameter**


#### Test your interop with Xamarin in Blazor

Don't forget to add your Blazor implementation in the dependency services of your Blazor app.
In your **Startup.cs** file of your **Blazor project**:

```csharp
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IXamarinBridge, XamarinBridgeProxy>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
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

### Detecting Runtime Platform

In order to detect the current runtime environment of your Blazor app within Blazor, you must set the following in your **Startup.cs** file of your Blazor project:

```csharp

//SOME CODE
br.AddComponent<App>("app");

BlazorWebViewService.Init(br, "blazorXamarin", (bool success) =>
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
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>BlazorMobile.BlazorApp</title>
    <base href="/" />
    <link href="css/bootstrap/bootstrap.min.css" rel="stylesheet" />
    <link href="css/site.css" rel="stylesheet" />
</head>
<body>
    <app>Loading...</app>
    <script type="text/javascript" src="js/blazor.polyfill.js"></script>
    <script src="_framework/components.webassembly.js"></script>
    <blazorXamarin></blazorXamarin>
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
**Device.RuntimePlatform** namespace is **BlazorMobile.Common** and does mimic the result of **Xamarin.Forms.Device.RuntimePlatform** with some minor change, as **Browser** is returned in a pure web app in a browser, and **Unknown** is returned if an error occur or if the initialization is not yet made.
