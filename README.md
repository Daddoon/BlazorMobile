# BlazorMobile[<img src="logo_blazormobile_256x256.png?raw=true" align="right" width="200">]() 

Create full C# driven hybrid-apps for iOS, Android, UWP & Desktop with Blazor!

**BlazorMobile** - is a set of Nuget packages & project templates for embedding a Blazor web application as a standalone mobile application, hosted in Xamarin.

## Framework requirement


- **Blazor 3.2.0**
- **.NET Core 3.1**

## Platform requirements
 
- **Android:** 4.4 or greater
- **iOS:** 12.0 or greater
- **UWP:** Build 16299 or greater
- **Windows:** Electron.NET
- **Linux:** Electron.NET
- **macOS:** Electron.NET

### Additional platform notes

#### Android support

- No support of **Android App Bundles** (AAB) on **API 28 in release mode** - Credits to [@shawndeggans](https://github.com/shawndeggans) - [See #137 for more info](https://github.com/Daddoon/BlazorMobile/issues/137)
- Considering the previous statement, consider releasing your app as an **APK**. See also the [Android Build size optimization](#android-build-size-optimization) section.

#### Universal Windows Platform
- BlazorMobile has been tested working on **Windows 10**! - minimum version: **10.16299**
- BlazorMobile has been tested working on **Xbox One**! - minimum version: **10.18362**

#### ElectronNET support
- **.NET Core 3.1** is required on your ElectronNET application.
- See [Electron.NET support with BlazorMobile](#electronnet-support-with-blazormobile) section for more info.

## Summary

- [Difference between BlazorMobile & Progressive Web Apps (PWA)](#difference-between-blazormobile--progressive-web-apps-pwa)
- [Getting started from sample](#getting-started-from-sample)
- [Linking your Blazor app to your Xamarin project](#linking-your-blazor-app-to-your-xamarin-project)
- [Detecting Runtime Platform](#detecting-runtime-platform)
- [Communication between Blazor & Native](#communication-between-blazor--native)
- [Validating the Blazor application navigation](#validating-the-blazor-application-navigation)
- [Device remote debugging & Debugging from NET Core 3.0](#device-remote-debugging--debugging-from-net-core-30)
- [Loading several or external BlazorMobile apps](#loading-several-or-external-blazormobile-apps)
- [Android Build size optimization](#android-build-size-optimization)
- [Electron.NET support with BlazorMobile](#electronnet-support-with-blazormobile)

## Troubleshoot

- [My Xamarin services are not found when interoping in UWP](#my-xamarin-services-are-not-found-when-interoping-in-uwp)
- [Cannot connect to a remote webserver on UWP](#cannot-connect-to-a-remote-webserver-on-uwp)
- [Unable to connect to UWP remotely even with NetworkIsolation disabled](#unable-to-connect-to-uwp-remotely-even-with-networkisolation-disabled)
- [Cyclic restore issue at project template creation](#cyclic-restore-issue-at-project-template-creation)
- [iOS/Safari 13: Unhandled Promise Rejection: TypeError: 'arguments', 'callee', and 'caller' cannot be accessed in this context](#iossafari-13-unhandled-promise-rejection-typeerror-arguments-callee-and-caller-cannot-be-accessed-in-this-context)
- [ITMS-90809: Deprecated API Usage - Apple will stop accepting submissions of apps that use UIWebView APIs](#itms-90809-deprecated-api-usage---apple-will-stop-accepting-submissions-of-apps-that-use-uiwebview-apis)
- [Apple Rejection - Your app uses or references the following non-public APIs: LinkPresentation.framework, QuickLookThumbnailing.framework](#apple-rejection---your-app-uses-or-references-the-following-non-public-apis-linkpresentationframework-quicklookthumbnailingframework)
- [Android crash at boot on API 28](#android-crash-at-boot-on-api-28)
- [Application refresh and restart after going in background](#application-refresh-and-restart-after-going-in-background)

## Updates and Migrations guides

**[See the Migration guide page](MIGRATION.md)**

## Difference between BlazorMobile & Progressive Web Apps (PWA)

Both creating an application as PWA or using BlazorMobile can be an option with Blazor

The main differences / advantages of BlazorMobile are:

- Access to native

- Access from Web to native both in C#

- More control about your application behaviors, depending your needs and complexity, some type of integration may be difficult with PWA. Still i think the majority of things can be done with PWA only.

- You can support old versions of Android where WebAssembly was even not present. Actually because the WebView component used in the plugin is the excellent Mozilla GeckoView instead, so giving you some consistency accross Android devices. On the other side, PWA will never work on older devices, because of lack of PWA support, or because the browser implementation of the system does not have any support of WebAssembly, required by Blazor.

- If you are good at designing your application, you can even make your application PWA and BlazorMobile compatible, as you can work intensively with DependencyInjection for services, and so, have multiple implementations of your app services in one or another use case !

## Getting started from sample

First install the template model with the following command from a command prompt:

```console
dotnet new -i BlazorMobile.Templates::3.2.8
```

Then go the folder where you want your project to be created, and from a command prompt type the following command, and of course replace **MyProjectName** to your desired project name:

```console
dotnet new blazormobile -n MyProjectName
```

If you plan to also use the Desktop project using Electron.NET, you must first execute this command in order to install the Electron tool on your system:

```console
dotnet tool install ElectronNET.CLI --version 5.30.1 -g
```

Then from your Desktop project directory, execute the following command:

```console
electronize init
```

Open you newly created solution, and you are good to go!

## Linking your Blazor app to your Xamarin project

### Getting started from a fresh install

Beginning from a freshly installed BlazorMobile template, **everything is already set by default.**

The following informations only explains how your Xamarin.Forms project load your Blazor WebAssembly application.

### How it works

**<ins>This are informational bits about the project structure:</ins>**

- If you plan to use the **BlazorMobile platforms only** (iOS, Android, UWP):
  - You can follow the structure in this guide, embedding your Blazor app in the Xamarin.Forms shared project
- If you plan to use the **ElectronNET platform** in addition of BlazorMobile platforms:
  - Embed your Blazor app package in an assembly outside the Xamarin.Forms shared project, and call the package
  registration, with **WebApplicationFactory.RegisterAppStreamResolver**, in each device root project.
  - This is not mandatory but **highly recommended**, as this configuration prevent to ship your BlazorMobile application twice
  on ElectronNET, otherwise it would contain the packaged app, not used with ElectronNET implementation, and the regular app project.

**In order to ship your Blazor application within your Xamarin apps, you need to pack it and make it available to it.**

Your Blazor app will be automatically packaged thanks to the **BlazorMobile.Build** NuGet package, that must be already installed on your Blazor web application project. The package location will be written in the build output after the Blazor build mecanism.

The filename should be **YourBlazorProjectName.zip**.

The steps to easily link it in Xamarin:

- Add your package **as a link** in your Xamarin.Forms shared project, formerly **YourAppName**, from the Blazor web app bin directory.

- Set the property of your package file as an **Embedded Resource** from Visual Studio property window.

- **Recommended**: Add a dependency on your Xamarin.Forms shared project, and tick your Blazor web application as a build dependency. **This way you will be assured that even if there is no direct reference between the Xamarin.Forms shared project and the blazor web application assembly, the blazor project and the zip are always updated before building your mobile application project**.

- Set the path to your package in your Xamarin.Forms shared project. In the **App.xaml.cs** file, set the path in your **RegisterAppStreamResolver** delegate.

As seen on the **BlazorMobile.Sample** project, assuming a file linked in a virtual folder called **Package**, we would have a code like this:

```csharp
namespace BlazorMobile.Sample
{
	public partial class App : BlazorApplication
	{
        public const string BlazorAppPackageName = "BlazorMobile.Sample.Blazor.zip";

        public App()
        {
            InitializeComponent();

            //Some code

            //Register Blazor application package resolver
            WebApplicationFactory.RegisterAppStreamResolver(() =>
            {
                //This app assembly
                var assembly = typeof(App).Assembly;

                //Name of our current Blazor package in this project, stored as an "Embedded Resource"
                //The file is resolved through AssemblyName.FolderAsNamespace.YourPackageNameFile

                //In this example, the result would be BlazorMobile.Sample.Package.BlazorMobile.Sample.Blazor.zip
                return assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Package.{BlazorAppPackageName}");
            });

            //Some code

            MainPage = new MainPage();
        }
    }
}
```

## Detecting Runtime Platform

Just call:

```csharp
BlazorDevice.RuntimePlatform
```

In order to retrieve the current device runtime platform.

Note that the **BlazorMobilService.Init()** has an **onFinish** callback delegate. Every call to **BlazorDevice.RuntimePlatform** before the onFinish delegate call will return **BlazorDevice.Unkown** instead of the detected platform.

## Communication between Blazor & Native

### <ins>Using the ProxyInterface API</ins>
**ProxyInterface API usages and limitations:**

- Blazor to Xamarin communication
- Unidirectional workflow: The Blazor application is always the call initiator
- Ideal for Business logic with predictable results
- API calls from Blazor to native are awaitable from a Task object
- Usage of interface contracts

#### How-to use

**In the project shared between Blazor & Xamarin**, formerly **YourAppName.Common** create an interface, and add the **[ProxyInterface]** attribute on top of it. Assuming using the sample **IXamarinBridge** interface, present by default on YourAppName.Common project, your interface may look like this:

```csharp
using BlazorMobile.Common.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorMobile.Sample.Common.Interfaces
{
    [ProxyInterface]
    public interface IXamarinBridge
    {
        Task<List<string>> DisplayAlert(string title, string msg, string cancel);
    }
}

```

**In your Xamarin shared application project**, formerly **YourAppName** project:

- Create your implementation class
- Inherit your previously created interface on this class
- Implement your native code behavior
- Call **DependencyService.Register** and register your interface and class implementation during your application startup.

**NOTE: If you plan to ship on UWP, the last statement is important.** Because if using the Xamarin attribute method instead,
UWP native toolchain will strip your services registration when compiling in Release mode with .NET Native.

It's so highly recommended to keep the **DependencyService.Register** route.
	
Your implementation may look like this. Here a kind of simple example:

```csharp
using BlazorMobile.Sample.Common.Interfaces;
using BlazorMobile.Sample.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.Sample.Services
{
    public class XamarinBridge : IXamarinBridge
    {
        public async Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            await App.Current.MainPage.DisplayAlert(title, msg, cancel);

            List<string> result = new List<string>()
            {
                "Lorem",
                "Ipsum",
                "Dolorem",
            };

            return result;
        }
    }
}
```

**In your Blazor project**, you only have two things to do:

- Call **services.AddBlazorMobileNativeServices\<Startup\>();** from **ConfigureServices** in **Startup.cs**
- Inject your interface in your pages and call the methods whenever you want!

Starting from the template, as a convinience, adding BlazorMobile natives services from **ServicesHelper.ConfigureCommonServices**.

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace BlazorMobile.Sample.Blazor.Helpers
{
    public static class ServicesHelper
    {
        public static void ConfigureCommonServices(IServiceCollection services)
        {
            //Add services shared between multiples project here
            services.AddBlazorMobileNativeServices<Startup>();
        }
    }
}
```

Then if you want to use any of your Blazor to native interface, it's as simple as this:

```csharp
@page  "/blazormobile"

@using BlazorMobile.Common
@using BlazorMobile.Sample.Common.Interfaces
@inject IXamarinBridge XamarinBridge

<h1>BlazorMobile</h1>

<button class="btn btn-primary" @onclick="@ShowPlatform">Show Runtime platform in a native dialog</button>

@code {

    async void ShowPlatform()
    {
        await XamarinBridge.DisplayAlert("Platform identity", $"Current platform is {BlazorDevice.RuntimePlatform}", "Great!");
    }
}
```

### <ins>Using the Message API</ins>
**Message API usages and limitations:**

- Xamarin to Blazor communication only with **CallJSInvokableMethod** method
- Xamarin to Blazor & Blazor to Blazor with **PostMessage** method
- Message broadcasting only
- Unidirectional workflow: The message sender cannot wait for any return value
- Ideal for Business logic with unpredictable results: The native side can post messages to the Blazor application, according to some external events
- API calls are not awaitable
- **PostMessage** only: Messages are limited to one parameter
- **PostMessage** only: Allow to forward messages to static & instanciated method delegates
- **CallJSInvokableMethod** only: Messages can have any parameters, but should match the expected method signature
- **CallJSInvokableMethod** only: Allow to forward message to static JSInvokable methods only

#### How-to use

<ins>**CallJSInvokableMethod** - Allow to call a Blazor static JSInvokable method</ins>

From the **IBlazorWebView** object retrieved when launching your BlazorMobile application, you should be able to call **CallJSInvokableMethod**.
This is self explanatory about it's usage, as it look like signature you can find on the InvokeAsyncMethod in Javascript in a regular Blazor application.

```csharp
/// <summary>
/// Call a static JSInvokable method from native side
/// </summary>
/// <param name="assembly">The assembly of the JSInvokable method to call</param>
/// <param name="method">The JSInvokable method name</param>
/// <param name="args">Parameters to forward to Blazor app. Check that your parameters are serializable/deserializable from both native and Blazor sides.</param>
/// <returns></returns>
void CallJSInvokableMethod(string assembly,string method, params object[] args);
```

An usage could look like this:

```csharp
webview = BlazorWebViewFactory.Create();

//Assuming that we know that the Blazor application already started
webview.CallJSInvokableMethod("MyBlazorAppAssemblyName", "MyJSInvokableMethodName", "param1", "param2", "param3");
```

Your JSInvokable method on Blazor side will then be called.

<ins>**PostMessage** - Allow to post a message to a static or instanciated delegate method</ins>

This API is in two parts, one in the native side, the other one on the Blazor side.

Messages sent will be received:

- Only by the Blazor side
- And forwarded to delegates methods registered with a **matching message name** to listen and **matching expected parameter type**

##### Native side
From the **IBlazorWebView** object retrieved when launching your BlazorMobile application, you should be able to call **PostMessage**.

```csharp
/// <summary>
/// Post a message to the Blazor app. Any objects that listen to a specific message name by calling BlazorMobileService.MessageSubscribe will trigger their associated handlers.
/// </summary>
/// <param name="messageName"></param>
/// <param name="args"></param>
void PostMessage<TArgs>(string messageName, TArgs args);
```

An usage could look like this:

```csharp
webview = BlazorWebViewFactory.Create();

//Assuming that we know that the Blazor application already started
webview.PostMessage<string>("myNotification", "my notification value");
```

Your message will be sent to the Blazor app.

##### Blazor side

In order to receive message notifications on Blazor side, you should subscribe to the message to listen, with the expected argument type.
Here are the three static methods usable from the **BlazorMobileService** static class in the Blazor app, for Message API:

```csharp
/// <summary>
/// Subscribe to a specific message name sent from native side with PostMessage, and forward the event to the specified delegate if received
/// </summary>
/// <param name="messageName">The message name to subscribe to</param>
/// <param name="handler">The delegate action that must be executed at message reception</param>
static void MessageSubscribe<TArgs>(string messageName, Action<TArgs> handler);

/// <summary>
/// Unsubscribe to a specific message name sent from native side with PostMessage
/// </summary>
/// <param name="messageName">The message name to unsubscribe to</param>
/// <param name="handler">The delegate action that must be unsubscribed</param>
static void MessageUnsubscribe<TArgs>(string messageName, Action<TArgs> handler);

/// <summary>
/// Allow to post a message to any delegate action registered through MessageSubscribe.
/// This method behavior is similar to the IBlazorWebView.PostMessage method on the native side,
/// except that you send message from within your Blazor app instead sending it from native side.
/// </summary>
/// <typeparam name="TArgs">The paramter expected type</typeparam>
/// <param name="messageName">The message name to target</param>
/// <param name="value">The value to send in the message</param>
static void PostMessage<TArgs>(string messageName, TArgs value);
```

In order to receive the message sent from the native side in our previous example we could do this in a Blazor page:

```csharp
public void OnMessageReceived(string payload)
{
   //Stuff here will be called when receiving the message
}

BlazorMobileService.MessageSubscribe<string>("myNotification", OnMessageReceived);
```

**NOTE:** If you are subscribing an instance method member like in this example, to the MessageSubscribe method,
**it's highly recommended** to cleanly unregister it when you know that your object instance will be disposed.

In this example, from a **Razor** page you could do something like this:

```csharp
@page "/myPage"
@implements IDisposable

//Some code

@code {
    //Some code

    public void Dispose()
    {
        BlazorMobileService.MessageUnsubscribe<string>("myNotification", OnMessageReceived);
    }
}
```

If there is no **IDisposable** mecanism on the C# component you are working on, you may also just unregister at **Destructor** level.
See the following example:

```csharp
public class MyClass()
{
    public void OnMessageReceived(string payload)
    {
       //Stuff here will be called when receiving the message
    }

    //Constructor
    MyClass()
    {
        BlazorMobileService.MessageSubscribe<string>("myNotification", OnMessageReceived);
    }

    //Destructor
    ~MyClass()
    {
        BlazorMobileService.MessageUnsubscribe<string>("myNotification", OnMessageReceived);
    }
}
```

## Validating the Blazor application navigation

Rules have been enforced and/or implemented on each supported platforms, in order to allow:

- Control of the document navigation in the **main frame** of your Blazor application
- Control of the document navigation in **subframes** of your Blazor application

This mean that if you register to the **Navigating** event handler of your **IBlazorWebView** object, you should be able to
allow or cancel any document navigation made by:

- Your application page (main frame) navigating
- Your application page (main frame) trying to open a new window
- An iframe of your application page (subframe) navigating
- An iframe of you application page (subframe) trying to open a new window

As your Blazor application is mainly a single webview, you may want to prevent any unexpected action that would make your
page navigate, and/or control more precisely some iframe behaviors shown in your Blazor application.

With this Navigating enforcement, you should be able to easily block any unwanted navigation, and instead opening the requested
url in an external browser, as an example.

Here is a sample code you find with the default template of BlazorMobile.

In your **MainPage.xaml.cs** file:

```csharp
//Blazor WebView agnostic contoller logic
webview = BlazorWebViewFactory.Create();

//Manage your native application behavior when an external resource is requested in your Blazor web application
//Customize your app behavior in BlazorMobile.Sample.Handler.OnBlazorWebViewNavigationHandler.cs file or create your own!
webview.Navigating += OnBlazorWebViewNavigationHandler.OnBlazorWebViewNavigating;
```

This register the navigating event to the **OnBlazorWebViewNavigating** event handler, included in the sample.

Here is the control code in the sample:

```csharp
using BlazorMobile.Common;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xamarin.Forms;

namespace BlazorMobile.Sample.Handler
{
    public static class OnBlazorWebViewNavigationHandler
    {
        public static void OnBlazorWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            var applicationBaseURL = WebApplicationFactory.GetBaseURL() + "/";

            if (e.Url.Equals(applicationBaseURL, StringComparison.OrdinalIgnoreCase))
            {
                //This is our application base URI. We should do nothing and continue navigating to the app
                e.Cancel = false;
            }
            else if (e.Url.StartsWith(WebApplicationFactory.GetBaseURL(), StringComparison.OrdinalIgnoreCase))
            {
                //Here, our application is loading an URI
                //You may add a custom logic, like opening a new view, changing the URI parameters then opening a view...

                e.Cancel = true;

                switch (BlazorDevice.RuntimePlatform)
                {
                    default:
                        Device.OpenUri(new Uri(WebUtility.UrlDecode(e.Url)));
                        break;
                }
            }
            else
            {
                //If here this is not our application loading an URI
                //You may add a custom logic, like opening a new view, changing the URI parameters then opening a view...

                e.Cancel = true;

                switch (BlazorDevice.RuntimePlatform)
                {
                    default:
                        Device.OpenUri(new Uri(WebUtility.UrlDecode(e.Url)));
                        break;
                }
            }
        }
    }
}
```

## Device remote debugging & Debugging from NET Core 3.0

Even if there is now some debug functionalities in the Blazor WASM version in Chrome, it is pretty limited compared to the pure server-side debugging with NET Core 3.0.

A small server-side Blazor application sample has been added in order to test and debug your code from it. See your **Blazor.Server** project.
You don't have to code anything in it, as it will use all the code logic you have done with the **Blazor** project (the WASM one).

This is very usefull if you need to debug your Blazor application logic, and also your device.

Credits to **@Suchiman**,  for the [BlazorDualMode](https://github.com/Suchiman/BlazorDualMode) project, taken as reference for server sharing client-side Blazor model.

_**"But wait ! I cannot ship a server-side version of my Blazor application as a mobile app !"**_

Of you course you can't. But you can do remote debugging on your device in order to mimic your mobile application environment, from your development environment.

**You should be able:**

- To test, debug, inspect from your PC with the NET Core (Server side version)
- Get all your real device informations and behaviors, while debugging on your PC.
- Also validate the WASM version behavior from your PC

**You won't be able:**

- To validate any specific / faulty behavior due to the device browser

For this last critical point, you should remember that you may have some tools shipped for device browser debugging.
On **iOS**, you should debug from **Safari on OSX** (see online documentation), and on **Android**, you should debug from the **WebIDE** tool available on **Firefox 67** (SHIFT+F8 on Windows). New debugging tooling available from more recent Firefox version are not yet available for current GeckoView version, and WebIDE has been removed on more recent versions of Firefox.

### Enable remote debugging

There is some, but little configuration to make in order to allow remote debugging.

#### Xamarin side

On the Xamarin side, you must allow debug features in order to allow external source to connect to your Device.
On the **BlazorMobile.Sample** project, in **App.cs** constructor, we will allow debug features. see:

```csharp
public App()
{
    ...

    #if DEBUG
    //This allow remote debugging features
    WebApplicationFactory.EnableDebugFeatures();
    #endif

    WebApplicationFactory.SetHttpPort(8888);
    ...
}
```

Also note the initialization and usage of the **8888** port. You may and want to use any other valid port. Just keep in mind the current used port in your application, for the remote debugging.

**NOTE:** The current **#if DEBUG** directive is present by default in source project, but it seem that it is removed when creating project template package.

#### Blazor side

On the Blazor project, both on **WASM** and **Server** projects if you want to test on both, you must call **BlazorService.EnableClientToDeviceRemoteDebugging** in your **Statup.cs**, **Configure** method. see:
```csharp
using BlazorMobile.Common;
using BlazorMobile.Common.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using BlazorMobile.Sample.Blazor.Helpers;

namespace BlazorMobile.Sample.Blazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            ServicesHelper.ConfigureCommonServices(services);
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            #region DEBUG

            //Only if you want to test WebAssembly with remote debugging from a dev machine
            BlazorMobileService.EnableClientToDeviceRemoteDebugging("192.168.1.118", 8888);

            #endregion

            BlazorMobileService.Init(app, (bool success) =>
            {
                Console.WriteLine($"Initialization success: {success}");
                Console.WriteLine("Device is: " + Device.RuntimePlatform);
            });

            app.AddComponent<MobileApp>("app");
        }
    }
}
```

**NOTE:** You must call **BlazorMobileService.EnableClientToDeviceRemoteDebugging** before the **BlazorMobileService.Init** call !

Replace of course the first parameter by your own **device IP address**, and use the **same port** as configured in your Xamarin project.

#### Additional configuration for UWP

On UWP, because of the NetworkIsolation behavior, you cannot connect by default from your PC to the UWP app.

You must execute this command in background during development in order to allow incoming remote connection, and so remote debugging, in your UWP app:

```
CheckNetIsolation loopbackexempt -is -n=YourUWPPackageFamilyName
```

Of course, replace **YourUWPPackageFamilyName** by your package name on UWP. You can find it in the **Packages** tab of your **Package.appxmanifest**, at the end.

#### Deploy & Launch mobile application, debug from PC

Then, you just need to deploy your application to your phone, and launch it in order to allow external source to connect to it.
You may just launch it on the device, and only debug Blazor from your PC, or you may also launch it with the Xamarin debugger, in order to test Xamarin code during Blazor session.

If you want to debug both Blazor side and Xamarin side you may:

- Open two Visual Studio instances, one for launching debug on the Xamarin project on your device, and the other instance for debugging the Blazor application.
- Use only one Visual Studio instance, and set your solution in multiple-project debugging mode.

The Blazor application will be launched from your PC, and it will try to connect to the remote application instance.

Values from Xamarin context will be returned, and your code will behave as launched within the device.

**NOTE:** You need to register **server_index** instead of **index.html** on your Blazor.Server project (even in Desktop project if needed), in **Startup.cs** in order to debug from the NET Core version.

The **server_index.cshtml** file is automatically generated by **BlazorMobile.Build**, in order to have a synced copy of your Blazor application index.html, **replacing blazor.webassembly.js to blazor.server.js** and also adding required component to load since **Blazor preview9**.

In **Blazor server** and **Desktop** projects, your **Startup.cs** must have this:

```csharp
endpoints.MapFallbackToPage("/server_index");
```

Instead of this:

```csharp
endpoints.MapFallbackToClientSideBlazor<BlazorMobile.Sample.Blazor.Startup>("index.html");
```

If you are starting from a fresh template, everything is right by default.

The server project should listen on http://localhost:5080/ by default.

When the server console will show up during your debugging session, you need to open a tab in your favorite browser and browse http://localhost:5080/ url, in order to connect and debug your Blazor .NET Core application.

## Loading several or external BlazorMobile apps

You can load several apps during your application lifetime, shipped at build time or loaded through external source or saved on your filesystem.
You can either manage by yourself how the packages are loaded and saved, or use some integrated helpers to store the packages if you want to, on your device, for all platforms.

The API is pretty straightforward, here is the availables methods through the **WebApplicationFactory** static class available from native side:

```csharp
/// <summary>
/// Add the given Stream as a package in a data store on the device, with the given name
/// </summary>
/// <param name="name"></param>
/// <param name="content"></param>
WebApplicationFactory.AddPackage(string name, Stream content);

/// <summary>
/// Remove the package with the given name from the data store of the device
/// </summary>
/// <param name="name"></param>
/// <returns></returns>
WebApplicationFactory.RemovePackage(string name);

/// <summary>
/// List available packages in the data store of the device
/// </summary>
/// <returns></returns>
WebApplicationFactory.ListPackages();

/// <summary>
/// Load an app package with the given name, stored on the device.
/// This is a shorthand on calling yourself <see cref="RegisterAppStreamResolver"/> and <see cref="ReloadApplication"/>
/// as the loading is managed by all the entries you get through <see cref="AddPackage(string, Stream)"/>, <see cref="RemovePackage(string)"/>, <see cref="ListPackages"/>.
/// </summary>
/// <param name="name">The package to load</param>
/// <returns></returns>
WebApplicationFactory.LoadPackage(string name);

/// <summary>
/// Load an app package from the given Stream object. You are responsible for your Stream management.
/// This mean that things may behave incorrectly if you close or dispose the stream. The given stream will
/// be automatically disposed if you load or register another package instead.
/// </summary>
/// <param name="appStream">The package to load as a Stream</param>
/// <returns></returns>
WebApplicationFactory.LoadPackageFromStream(Stream appStream);

/// <summary>
/// Load an app package with the given package assembly path if you have stored it the assembly as a static resource.
/// This is the default mode used at start when shipping your base application from BlazorMobile template, but you can
/// extend this in order to load different packages at your native app startup.
/// </summary>
/// <param name="packageAssembly">The assembly where your Blazor package is stored.
/// TIPS: If you know a type stored in this assembly, you may resolve the assembly object with 'typeof(YourType).Assembly'</param>
/// <param name="packagePath">The relative path where the Blazor package is stored in the assembly</param>
/// <returns></returns>
WebApplicationFactory.LoadPackageFromAssembly(Assembly packageAssembly, string packagePath);
```

## Android Build size optimization

The underlying Webview component used with BlazorMobile on Android is the excellent **Mozilla GeckoView** browser component, replacing the traditional Webview component shipped with the OS.
This component allow us to:

- Having WebAssembly available even on Android version that does not support it
- Having a consistent Webview component accross recent and old Android versions.

This with the downside that we ship the GeckoView component in the APK. Without any optimzations, this component take roughly **150 MB** because it ship all the CPU implementations by default.

The solution to this problem is to ship one APK per ABI, as this will split the multiple ABI implementation of the GeckoView component to each specific APK ABI.
The GeckoView component for Android in your APK will then respectively shrink to approximatively **50MB** per platform.

**<u>Recommended readings:</u>**

- Microsoft documentation about [Building ABI-Specific APKs](https://docs.microsoft.com/en-us/xamarin/android/deploy-test/building-apps/abi-specific-apks).
- Google Play documentation about [Multiple APK support](https://developer.android.com/google/play/publishing/multiple-apks)
- Google Play [64-bit mandatory publishing since 1st August 2019](https://developer.android.com/distribute/best-practices/develop/64-bit).
- As stated in [this section](https://developer.android.com/distribute/best-practices/develop/64-bit#multi-apk-compliance) of the previous article, one important information to know coming from a Xamarin APK release is this:

```
Multi-APK and 64-bit compliance

If you are using Google Plays multiple-APK support to publish your app, note that compliance with the 64-bit requirement is evaluated at the release level. However, the 64-bit requirement does not apply to APKs or app bundles that are not distributed to devices running Android 9 Pie or later.
If one of your APKs is marked as not being compliant, but is older and its not possible to bring it into compliance, one strategy is to add a maxSdkVersion="27" attribute in the uses-sdk element in that APKs manifest. This APK wont be delivered to devices running Android 9 Pie or later, and it will no longer block compliance.
```

## Electron.NET support with BlazorMobile

You can also deploy your application developped with Blazor & BlazorMobile as a desktop application with **Electron.NET**.
The plugin has been updated in order to be aware of an Electron.NET executing context and behave correctly, with your same codebase and project structure.

Be aware that even if a Xamarin.Forms library is present on the Electron.NET desktop application, there is no deep support of the Xamarin.Forms API.

If you need to call anything from Xamarin on your shared Xamarin.Forms project that is not supported yet, you can check if we are running through Electron or Xamarin, by calling **BlazorDevice.IsElectronNET()**.

To get started about the Electron.NET Desktop project, it's highly recommended to create it from **BlazoreMobile.Templates**. See [Getting started from sample](#getting-started-from-sample) section.

### Xamarin.Forms support on Electron.NET

- **DisplayAlert** - Like App.Current.MainPage.DisplayAlert(title, msg, cancel);
- **DisplayActionSheet** - Like App.Current.MainPage.DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);
- **Device.OpenUri**
- **Navigating** events - On **IBlazorWebView** only
- **DependencyService** service class
- **Device.RuntimePlatform** will return "ElectronNET".
- **BlazorDevice.RuntimePlatform** will returns regular Xamarin.Forms values, with in addition **Windows**, **Linux**. Consts values available on **BlazorDevice** for RuntimePlatforms comparison have been updated to all theses values.

**NOTE:** BlazorDevice.RuntimePlatform never returns "ElectronNET" but ElectronNET presence can be checked by **BlazorDevice.IsElectronNET**.

## Troubleshoot

### My Xamarin services are not found when interoping in UWP

As stated in the [Communication between Blazor & Native](#communication-between-blazor--native) section in **Using the ProxtInterface API**, check that your services are registered through the **DependencyService.Register** method call.

```
If you plan to ship on UWP [...] if using the Xamarin attribute method instead, UWP native toolchain will strip your services registration when compiling in Release mode with .NET Native.
```

### Cannot connect to a remote webserver on UWP

There is some behaviors that are specifics to UWP:

- You cannot connect to a local webserver / socket endpoint, out of process, from the same machine. This mean that if you are doing tests about webservices from IIS, Kestrel or other from the same computer, UWP will be unable to connect to them. The server must be present on an other machine.


- If you are doing any web requests with HTTPS, UWP will block them if the certificate is self-signed or not trusted, as it follow the Edge browser security policy. You may not override this behavior from the Webview component, but you may override it if your are doing your requests from the native side instead as you may have more control about web requests behavior, but this less ideal from a design point of view.

### Unable to connect to UWP remotely even with NetworkIsolation disabled

This behavior may happen if the certificate used in your **Package.appxmanifest** is not present on your computer. This may likely happen starting from the template.
Please read this Microsoft documentation, [Create a certificate for package signing](https://docs.microsoft.com/en-us/windows/msix/package/create-certificate-package-signing), and don't forget to set your newly created certificate as your UWP certificate afterwards,
from your **Package.appxmanifest** property window.

### Cyclic restore issue at project template creation

This may happen if you called your project **BlazorMobile** at template creation, as it seem to confuse the NuGet restore command with the Nuget packages with the same suffix name, like **BlazorMobile** and **BlazorMobile.Common**.

Just avoid theses reserved names when creating your project.

### iOS/Safari 13: Unhandled Promise Rejection: TypeError: 'arguments', 'callee', and 'caller' cannot be accessed in this context

This error is actually a regression in iOS 13, preventing Blazor to boot correctly. Microsoft already fixed the bug preventing to boot, but it's not yet officialy released.
In the meantime, if you want to workaround this bug temporiraly, add this line...

```javascript
<script>var Module;</script>
```
...before the **blazor.webassembly.js** script tag.

Credits to [@kmiller68](https://github.com/kmiller68) in [this issue](https://github.com/mono/mono/issues/15588#issuecomment-529056521)

And also you have to opt-in to a **patched** blazor.webassembly.js runtime at iOS app startup.
If so, in your **AppDelegate.cs** file, before the **LoadApplication**, call **EnableDelayedStartPatch** like in this example:

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    global::Xamarin.Forms.Forms.Init();
    BlazorWebViewService.Init();

    //Register our Blazor app package
    WebApplicationFactory.RegisterAppStreamResolver(AppPackageHelper.ResolveAppPackageStream);

    if (int.TryParse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0], out int majorVersion) && majorVersion >= 13)
    {
        BlazorWebViewService.EnableDelayedStartPatch();
    }

    LoadApplication(new App());

    return base.FinishedLaunching(app, options);
}
```

### ITMS-90809: Deprecated API Usage - Apple will stop accepting submissions of apps that use UIWebView APIs

When submiting an iOS app on the AppStore you may have this message: **ITMS-90809: Deprecated API Usage - Apple will stop accepting submissions of apps that use UIWebView APIs . See https://developer.apple.com/documentation/uikit/uiwebview for more information.**

Please follow [this issue](https://github.com/xamarin/Xamarin.Forms/issues/7323) on Xamarin.Forms GitHub page, or [this documentation](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/webview?tabs=windows#uiwebview-deprecation-and-app-store-rejection-itms-90809)

As stated, at the date of writing:

> UIWebView Deprecation and App Store Rejection (ITMS-90809)
>
>Starting in April 2020, Apple will reject apps that still use the deprecated UIWebView API. While Xamarin.Forms has switched to WKWebView as the default, there is still a reference to the older SDK in the Xamarin.Forms binaries. Current iOS linker behavior does not remove this, and as a result the deprecated UIWebView API will still appear to be referenced from your app when you submit to the App Store.
>
>A preview version of the linker is available to fix this issue. To enable the preview, you will need to supply an additional argument --optimize=experimental-xforms-product-type to the linker.
>
>The prerequisites for this to work are:
>
>    Xamarin.Forms 4.5 or higher – Pre-release versions of Xamarin.Forms 4.5 can be used.
>    Xamarin.iOS 13.10.0.17 or higher – Check your Xamarin.iOS version in Visual Studio. This version of Xamarin.iOS is included with Visual Studio for Mac 8.4.1 and Visual Studio 16.4.3.
>    Remove references to UIWebView – Your code should not have any references to UIWebView or any classes that make use of UIWebView.

### Apple Rejection - Your app uses or references the following non-public APIs: LinkPresentation.framework, QuickLookThumbnailing.framework

You may receive this message from Apple:

```
Guideline 2.5.1 - Performance - Software Requirements

Your app uses or references the following non-public APIs:

- LinkPresentation.framework
- QuickLookThumbnailing.framework

The use of non-public APIs is not permitted on the App Store because it can lead to a poor user experience should these APIs change.

Continuing to use or conceal non-public APIs in future submissions of this app may result in the termination of your Apple Developer account, as well as removal of all associated apps from the App Store.
```

In this case:

- Set the linker setting to **Link Framework SDKs Only** on your iOS project
- And just for double-check this behavior as some Visual Studio version doesn't respect this option, add this into the **Additional mtouch arguments** input on your iOS project:

```shell
--linksdkonly --linkskip=LinkPresentation --linkskip=QuickLookThumbnailing
```

### Android crash at boot on API 28

This may be related if you are building your app as an **Android App Bundles** in release mode, API 28. Credits to [@shawndeggans](https://github.com/shawndeggans) - [See #137 for more info](https://github.com/Daddoon/BlazorMobile/issues/137).

As stated at the top of the documentation, consider releasing your app as an **APK**. See also the [Android Build size optimization](#android-build-size-optimization) section.

### Application refresh and restart after going in background

There is actually two cases when this behavior may happen:

- The app has been put in background, then foreground but the HTTP port of the webserver is not available anymore for whatever reason. As it can be problematic, the app restart on a new port

- The app started loading in the WebView, but you put the app in foreground and BlazorMobile initialization to native was not finished or Blazor WASM did not finish Blazor framework loading before getting put in background: In order to avoid inconsistent app state, the app restart.

- Another possible issue is if you put a regular Blazor app without inheriting from **MobileApp** component, and without calling the **BlazorMobilService.Init()** code at your app start. Theses calls notify to Native that it has loaded when it's finished. If they are not present they may fallback in the second point listed here as the app believe it has not loaded properly.

## Community

- **Azure DevOps Pipeline** by [@shawndeggans](https://github.com/shawndeggans) - [Download azure-pipelines.txt]("./Community/azure-pipelines.txt")
```
This script is meant to work within the DevOps Pipeline for the user project APK generation.
Presently, it only builds it to an archive, but I think I will eventually have it picked up and delivered to Genymotion On Demand.
```

## Authors

- **Guillaume ZAHRA** - [Daddoon](https://github.com/Daddoon) - Software Developer - from Joinville-le-Pont, France. Entrepreneur & founder of 2Bee SASU, working since 10 years with .NET and C#.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
