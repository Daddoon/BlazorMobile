# BlazorMobile[<img src="logo_blazormobile_256x256.png?raw=true" align="right" width="200">]() 

Create full C# driven hybrid-apps for iOS, Android, UWP & Desktop with Blazor!

**BlazorMobile** - is a set of Nuget packages & project templates for embedding a Blazor web application as a standalone mobile application, hosted in Xamarin.

## Framework requirement

- **Blazor:** 3.0.0-preview9.19465.2

## Platform requirements
 
- **Android:** 4.4 or greater
- **iOS:** 12.0 or greater
- **UWP:** Build 16299 or greater
- **Windows:** Electron.NET
- **Linux:** Electron.NET
- **macOS:** Electron.NET

### Additional platform notes

#### Universal Windows Platform
- UWP has been tested working on **Windows 10**! - minimum version: **10.16299**
- UWP has been tested working on **Xbox One**! - minimum version: **10.18362**

#### ElectronNET support
- **.NET Core 3.0** is required on your ElectronNET application.
- See [Electron.NET support with BlazorMobile](#electronnet-support-with-blazormobile) section for more info.

## Summary

- [Difference between BlazorMobile & Progressive Web Apps (PWA)](#difference-between-blazormobile--progressive-web-apps-pwa)
- [Getting started from sample](#getting-started-from-sample)
- [Linking your Blazor app to your Xamarin project](#linking-your-blazor-app-to-your-xamarin-project)
- [Detecting Runtime Platform](#detecting-runtime-platform)
- [Communication between Blazor & Native](#communication-between-blazor--native)
- [Validating the Blazor application navigation](#validating-the-blazor-application-navigation)
- [Device remote debugging & Debugging from NET Core 3.0](#device-remote-debugging--debugging-from-net-core-30)
- [Android Build size optimization](#android-build-size-optimization)
- [Electron.NET support with BlazorMobile](#electronnet-support-with-blazormobile)

## Troubleshoot

- [My Xamarin services are not found when interoping in UWP](#my-xamarin-services-are-not-found-when-interoping-in-uwp)
- [Cannot connect to a remote webserver on UWP](#cannot-connect-to-a-remote-webserver-on-uwp)
- [Unable to connect to UWP remotely even with NetworkIsolation disabled](#unable-to-connect-to-uwp-remotely-even-with-networkisolation-disabled)
- [System.ArgumentOutOfRangeException when calling Blazor to native](#systemargumentoutofrangeexception-when-calling-blazor-to-native)
- [Cyclic restore issue at project template creation](#cyclic-restore-issue-at-project-template-creation)
- [iOS/Safari 13: Unhandled Promise Rejection: TypeError: 'arguments', 'callee', and 'caller' cannot be accessed in this context](#iossafari-13-unhandled-promise-rejection-typeerror-arguments-callee-and-caller-cannot-be-accessed-in-this-context)
- [ITMS-90809: Deprecated API Usage - Apple will stop accepting submissions of apps that use UIWebView APIs](#itms-90809-deprecated-api-usage---apple-will-stop-accepting-submissions-of-apps-that-use-uiwebview-apis)

## Migration

- [BlazorMobile 0.8.0 to 3.0.3-preview7.19365.7](#blazormobile-080-to-303-preview7193657)
- [BlazorMobile 3.0.3-preview7.19365.7 to 3.0.4-preview7.19365.7](#blazormobile-303-preview7193657-to-304-preview7193657)
- [BlazorMobile 3.0.4-preview7.19365.7 to 3.0.5-preview8.19405.7](#blazormobile-304-preview7193657-to-305-preview8194057)
- [BlazorMobile 3.0.5-preview8.19405.7 to 3.0.6-preview8.19405.7](#blazormobile-305-preview8194057-to-306-preview8194057)
- [BlazorMobile 3.0.6-preview8.19405.7 to 3.0.7-preview8.19405.7](#blazormobile-306-preview8194057-to-307-preview8194057)
- [BlazorMobile 3.0.7-preview8.19405.7 to 3.0.8-preview8.19405.7](#blazormobile-307-preview8194057-to-308-preview8194057)
- [BlazorMobile 3.0.8-preview8.19405.7 to 3.0.9-preview8.19405.7](#blazormobile-308-preview8194057-to-309-preview8194057)
- [BlazorMobile 3.0.9-preview8.19405.7 to 3.0.10-preview9.19424.4](#blazormobile-309-preview8194057-to-3010-preview9194244)
- [BlazorMobile 3.0.10-preview9.19424.4 to 3.0.11-preview9.19465.2](#blazormobile-3010-preview9194244-to-3011-preview9194652)

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
dotnet new -i BlazorMobile.Templates::3.0.11-preview9.19465.2
```

Then go the folder where you want your project to be created, and from a command prompt type the following command, and of course replace **MyProjectName** to your desired project name:

```console
dotnet new blazormobile -n MyProjectName
```

If you plan to also use the Desktop project using Electron.NET, you must first execute this command in order to install the Electron tool on your system:

```console
dotnet tool install ElectronNET.CLI -g
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
On **iOS**, you should debug from **Safari on OSX** (see online documentation), and on **Android**, you should debug from **WebIDE** tool in **Firefox** (see online documentation).

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
            BlazorService.EnableClientToDeviceRemoteDebugging("192.168.1.118", 8888);

            #endregion

            BlazorService.Init(app, (bool success) =>
            {
                Console.WriteLine($"Initialization success: {success}");
                Console.WriteLine("Device is: " + Device.RuntimePlatform);
            });

            app.AddComponent<MobileApp>("app");
        }
    }
}
```

**NOTE:** You must call **BlazorService.EnableClientToDeviceRemoteDebugging** before the **BlazorService.Init** call !

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

Since **BlazorMobile 3.0.8-preview8.19405.7**, you can also deploy your application developped with Blazor & BlazorMobile as a desktop application with **Electron.NET**.
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

### Cannot connect to a remote webserver on UWP

There is some behaviors that are specifics to UWP:

- You cannot connect to a local webserver / socket endpoint, out of process, from the same machine. This mean that if you are doing tests about webservices from IIS, Kestrel or other from the same computer, UWP will be unable to connect to them. The server must be present on an other machine.


- If you are doing any web requests with HTTPS, UWP will block them if the certificate is self-signed or not trusted, as it follow the Edge browser security policy. You may not override this behavior from the Webview component, but you may override it if your are doing your requests from the native side instead as you may have more control about web requests behavior, but this less ideal from a design point of view.

### Unable to connect to UWP remotely even with NetworkIsolation disabled

This behavior may happen if the certificate used in your **Package.appxmanifest** is not present on your computer. This may likely happen starting from the template.
Please read this Microsoft documentation, [Create a certificate for package signing](https://docs.microsoft.com/en-us/windows/msix/package/create-certificate-package-signing), and don't forget to set your newly created certificate as your UWP certificate afterwards,
from your **Package.appxmanifest** property window.

### System.ArgumentOutOfRangeException when calling Blazor to native

This bug is a [regression from .NET Core 3.0-preview8](https://github.com/dotnet/corefx/issues/40409#issuecomment-522514553) that is already fixed for preview9.

We have to wait for preview9 shipping, or working on an older version of Blazor & BlazorMobile running on preview7, or compiling by yourself current Microsoft nightly builds.

In my opinion, the best option is to wait.

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

Please follow [this issue](https://github.com/xamarin/Xamarin.Forms/issues/7323) on Xamarin.Forms GitHub page.

## Community

- **Azure DevOps Pipeline** by [@shawndeggans](https://github.com/shawndeggans) - [Download azure-pipelines.txt]("./Community/azure-pipelines.txt")
```
This script is meant to work within the DevOps Pipeline for the user project APK generation.
Presently, it only builds it to an archive, but I think I will eventually have it picked up and delivered to Genymotion On Demand.
```

## Migration

### BlazorMobile 0.8.0 to 3.0.3-preview7.19365.7

In your Blazor project, edit your ***.csproj** file:

- Remove the **BlazorMobile.Common PackageReference**
- Remove the manual PostBuild event, that look like this:

```xml
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <Exec Command="rm $(ProjectDir)\BuildTools\artifacts\app.zip &gt;nul 2&gt;&amp;1&#xD;&#xA;$(ProjectDir)\BuildTools\7za.exe a $(ProjectDir)\BuildTools\artifacts\app.zip $(ProjectDir)wwwroot\* -mx1 -tzip&#xD;&#xA;$(ProjectDir)\BuildTools\7za.exe a $(ProjectDir)\BuildTools\artifacts\app.zip $(ProjectDir)$(OutputPath)dist\* -mx1 -tzip" />
</Target>
```
- In this same project file, add a PackageReference to **BlazorMobile.Build** and **BlazorMobile.Web**. This should look like this:

```xml
<ItemGroup>
  <PackageReference Include="BlazorMobile.Build" Version="3.0.3-preview7.19365.7" />
  <PackageReference Include="BlazorMobile.Web" Version="3.0.3-preview7.19365.7" />
  <PackageReference Include="Microsoft.AspNetCore.Blazor" Version="3.0.0-preview7.19365.7" />
  <PackageReference Include="Microsoft.AspNetCore.Blazor.Build" Version="3.0.0-preview7.19365.7" PrivateAssets="all" />
  <PackageReference Include="Microsoft.AspNetCore.Blazor.DevServer" Version="3.0.0-preview7.19365.7" />
</ItemGroup>
```

- In all of your projects, update any reference of **BlazorMobile** or **BlazorMobile.Common** to the **3.0.3-preview7.19365.7** version.

In your **Startup.cs** file, in **Configure**, replace:

```csharp
public void Configure(IComponentsApplicationBuilder app)
{
    app.AddComponent<App>("app");

    BlazorWebViewService.Init(app, "blazorXamarin", (bool success) =>
    {
        Console.WriteLine($"Initialization success: {success}");
        Console.WriteLine("Device is: " + Device.RuntimePlatform);
    });
}
```

to:

```csharp
public void Configure(IComponentsApplicationBuilder app)
{
    #if DEBUG

    //Only if you want to test WebAssembly with remote debugging from a dev machine
    BlazorService.EnableClientToDeviceRemoteDebugging("192.168.1.118", 8888);

    #endif

    BlazorService.Init(app, (bool success) =>
    {
        Console.WriteLine($"Initialization success: {success}");
        Console.WriteLine("Device is: " + Device.RuntimePlatform);
    });

    app.AddComponent<MobileApp>("app");
}
```

Actually, change the onSuccess delegate to anything you want.
But notice the **MobileApp** instead of **App** component.

You should create your own component inherited from **App**. Create a **MobileApp.cs** file in your Blazor project and copy/paste this:

```csharp
using BlazorMobile.Common.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMobile.Sample.Blazor
{
    public class MobileApp : App
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, nameof(BlazorMobileComponent));
            builder.OpenComponent(1, typeof(BlazorMobileComponent));
            builder.CloseComponent();
            builder.CloseElement();

            base.BuildRenderTree(builder);
        }
    }
}
```

Of course, replace the given namespaces by the one used by your own project.

- In your **index.html** from your Blazor project, you can safely remove the **blazorXamarin** tag.
- If you intent to use the server-mode to debug (see related documentation), you can also update the blazor script tag. In the current sample, **index.html** look like this:

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
    <script id="blazorMode"></script>
    <script>
        document.getElementById("blazorMode").src = window.location.search.includes("mode=server") ? "_framework/blazor.server.js" : "_framework/blazor.webassembly.js";
    </script>
</body> 
</html>
```

See the documentation, about how to switch from WASM to .NET Core debugging if needed.

- Update your **RegisterAppStreamResolver** code if needed. See the linking Blazor to Xamarin section for this.
- Add missing additionnals project if needed from samples, to your project.

New projects are:

- **BlazorMobile.Sample.Blazor.Server**, for testing your Blazor app with the .NET Core runtime
- **BlazorMobile.Sample.UWP**, for deploying your Blazor app to UWP (Windows 10).

### BlazorMobile 3.0.3-preview7.19365.7 to 3.0.4-preview7.19365.7

In your Xamarin shared project, like **BlazorMobile.Sample** sample project you should:

- Inherit from **BlazorApplication** instead of **Application** in **App.xaml.cs**

```csharp
using BlazorMobile.Components;
using BlazorMobile.Services;
using System;
using Xamarin.Forms;

namespace BlazorMobile.Sample
{
    public partial class App : BlazorApplication
    {
        public App()
        {
            ...Your code...
        }
    }
}
```

- Inherit from **BlazorApplication** instead of **Application** in **App.xaml** too. Your code should look like this:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<components:BlazorApplication
            xmlns:components="clr-namespace:BlazorMobile.Components;assembly=BlazorMobile"
            xmlns="http://xamarin.com/schemas/2014/forms"
            xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
            x:Class="BlazorMobile.Sample.App">
	<Application.Resources>
    
	</Application.Resources>
</components:BlazorApplication>
```

- You should remove any **WebApplicationFactory.StartWebServer** and **WebApplicationFactory.StopWebServer** reference in your **App.xaml.cs**, as they are now internals and managed by the **BlazorApplication** class. You can safely remove theses lines:

```csharp
protected override void OnStart()
{
    WebApplicationFactory.StartWebServer();
}

protected override void OnSleep()
{
    WebApplicationFactory.StopWebServer();
}

protected override void OnResume()
{
    WebApplicationFactory.ResetBlazorViewIfHttpPortChanged();
    WebApplicationFactory.StartWebServer();
}
```

**NOTE:** **WebApplicationFactory.SetHttpPort** is not mandatory anymore as if the app fail to bind on your specific port, it will fallback on another available port. But you can still use it for your specific needs and in order to assign a fixed port for remote debugging sessions.

### BlazorMobile 3.0.4-preview7.19365.7 to 3.0.5-preview8.19405.7

Nothing to do ! You only need to update your Blazor project according to [Blazor 3.0.0-preview8.19405.7 requirements](https://devblogs.microsoft.com/aspnet/asp-net-core-and-blazor-updates-in-net-core-3-0-preview-8/).

And of course, just update all your BlazorMobile.* NuGet packages to 3.0.5-preview8.19405.7

### BlazorMobile 3.0.5-preview8.19405.7 to 3.0.6-preview8.19405.7

Nothing to do ! Just update all your BlazorMobile.* NuGet packages to 3.0.6-preview8.19405.7

### BlazorMobile 3.0.6-preview8.19405.7 to 3.0.7-preview8.19405.7

You may update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.0.7-preview8.19405.7
```

Update all your BlazorMobile.* NuGet packages to 3.0.7-preview8.19405.7.

In all your project files replace all BlazorService class reference:

```charp
BlazorService
```

to

```csharp
BlazorMobileService
```

and all Device class reference from BlazorMobile assembly:


```charp
Device
```

to

```csharp
BlazorDevice
```

In your **BlazorMobileService.Init** calls, you should now remove the first argument.

This:

```csharp
app.UseEndpoints(endpoints =>
{
    var componentBuilder = endpoints.MapBlazorHub<MobileApp>("app");
    endpoints.MapDefaultControllerRoute();
    endpoints.MapFallbackToClientSideBlazor<BlazorMobile.Sample.Blazor.Startup>("server_index.html");

    BlazorService.EnableClientToDeviceRemoteDebugging("192.168.1.118", 8888);
    BlazorService.Init(componentBuilder, (bool success) =>
    {
        Console.WriteLine($"Initialization success: {success}");
        Console.WriteLine("Device is: " + Device.RuntimePlatform);
    });
});
```

Should now look like something like this:

```csharp
app.UseEndpoints(endpoints =>
{
    var componentBuilder = endpoints.MapBlazorHub<MobileApp>("app");
    endpoints.MapDefaultControllerRoute();
    endpoints.MapFallbackToClientSideBlazor<BlazorMobile.Sample.Blazor.Startup>("server_index.html");
});

BlazorMobileService.EnableClientToDeviceRemoteDebugging("192.168.1.118", 8888);
BlazorMobileService.Init((bool success) =>
{
    Console.WriteLine($"Initialization success: {success}");
    Console.WriteLine("Device is: " + BlazorDevice.RuntimePlatform);
});
```

As you can see, your code can now safely be written outside the UseEndpoints scope.

### BlazorMobile 3.0.7-preview8.19405.7 to 3.0.8-preview8.19405.7

Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.0.8-preview8.19405.7
```

Update all your BlazorMobile.* NuGet packages to 3.0.8-preview8.19405.7.

Then there is nothing to do, except if you created a template from the buggy **BlazorMobile 3.0.7-preview8.19405.7** version, as some things have been simplified since.

If you are in this case you must remove this line in **Startup.cs** of your Desktop project:

```csharp
Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
```

**BlazorMobile.Init** should be called before **UseBlazorMobileWithElectronNET**, in **Startup.cs** of your Desktop project:

```csharp
    BlazorMobileService.Init((bool success) =>
    {
	Console.WriteLine($"Initialization success: {success}");
	Console.WriteLine("Device is: " + BlazorDevice.RuntimePlatform);
    });

    app.UseBlazorMobileWithElectronNET<App>();
```

As the Xamarin.Forms initialization is now supported on ElectronNET environment, you must modify your **App.xaml.cs** in your shared Xamarin.Forms project, and remove theses lines:

```csharp
    //We do not need to configure any embedded HTTP server from here with Electron as we are already on ASP.NET Core
    //We do not need to set any package to load, nor loading any browser as it's already managed by Electron
    if (BlazorDevice.IsElectronNET())
    {
	return;
    }
```

In your **XamarinBridge.cs** test service, you do not need to check if **BlazorDevice.IsElectronNET** is true for DisplayAlert, as it has been implemented to forward to Electron. You can replace:

```csharp
public Task<List<string>> DisplayAlert(string title, string msg, string cancel)
{
    if (BlazorDevice.IsElectronNET())
    {
	Console.WriteLine(msg);
    }
    else
    {
	App.Current.MainPage.DisplayAlert(title, msg, cancel);
    }

    List<string> result = new List<string>()
    {
	"Lorem",
	"Ipsum",
	"Dolorem",
    };

    return Task.FromResult(result);
}
```

To something like this:

```csharp
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
```

### BlazorMobile 3.0.8-preview8.19405.7 to 3.0.9-preview8.19405.7

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.0.9-preview8.19405.7
```

- Update all your BlazorMobile.* NuGet packages to 3.0.9-preview8.19405.7.

- **Breaking changes:** All synchronous methods signatures from **MethodDispatcher.CallMethod** have been removed. You must only use **Task or Task<>** types signature, async or not, on your interop calls.

- Since **BlazorMobile 3.0.9-preview8.19405.7**, communication between Blazor to Native is automatic on the Blazor side.
You don't have to guess anymore the method signature needed for calling to native, nor creating yourself a proxy class with your interface.

- To upgrade with this new automated interoping behavior, you may now delete all your interface proxy **class implementation** in your Blazor project.

- In your **Startup.cs** file, in **ConfigureServices** or **ServicesHelper.ConfigureCommonServices** if you use something lik in the template model, call **services.AddBlazorMobileNativeServices\<Startup\>();**

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

- You may check the [Communication between Blazor & Native](#communication-between-blazor--native) section that has been updated in regard of this update.

### BlazorMobile 3.0.9-preview8.19405.7 to 3.0.10-preview9.19424.4

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.0.10-preview9.19424.4
```

- Update Blazor version to 3.0.0-preview9.19424.4. See [Blazor 3.0.0-preview9.19424.4 update requirements](https://devblogs.microsoft.com/aspnet/asp-net-core-and-blazor-updates-in-net-core-3-0-preview-9/).

- Update all your BlazorMobile.* NuGet packages to 3.0.10-preview9.19424.4.

- Remove **server_index.html** file from your client-side Blazor project.

- Compile your client-side Blazor project, even if it fail, it should generate a **server_index.cshtml**. In the property window set action to **None**.
Or alternatively, it should look like this in your Blazor project .csproj:

```xml
  <ItemGroup>
    <Content Remove="server_index.cshtml" />
  </ItemGroup>

  <ItemGroup>
    <None Include="server_index.cshtml" />
  </ItemGroup>
```

- In your **Blazor server** project and **Desktop** (ElectronNET) project if any, create a **Pages** folder at root level, like on the client-side project.

- Add **server_index.cshtml** from the Blazor client-side project as a link reference in this folder, and set its action in the property window to **Include**.
It should look like this in your Blazor server / Desktop projects, of course, **BlazorMobile.Sample.Blazor** replaced by **YourAppName.Blazor** :

```xml
   <ItemGroup>
     <Folder Include="Pages\" />
   </ItemGroup>
  
   <ItemGroup>
     <Content Include="..\BlazorMobile.Sample.Blazor\server_index.cshtml" Link="Pages\server_index.cshtml" />
   </ItemGroup>
```

- In your **Blazor server** project and **Desktop** (ElectronNET) project if any, in **Startup.cs** file:

**Remove**:

```csharp
endpoints.MapFallbackToClientSideBlazor<BlazorMobile.Sample.Blazor.Startup>("server_index.html");
```

**Replaced with**:

```csharp
endpoints.MapFallbackToPage("/server_index");
```

Here the [detailed reasons of this change](https://github.com/aspnet/AspNetCore/issues/13742)

### BlazorMobile 3.0.10-preview9.19424.4 to 3.0.11-preview9.19465.2

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.0.11-preview9.19465.2
```

- Update Blazor version to 3.0.0-preview9.19465.2.

- Update all your BlazorMobile.* NuGet packages to 3.0.11-preview9.19465.2.

- Add the **BlazorMobile.Build.Android** NuGet package to your Android project.

- In your shared project, you may add the new Navigating controller. See the default template.

If following the samples your **MainPage.xaml.cs** could now look like this:

```csharp
using BlazorMobile.Components;
using BlazorMobile.Sample.Handler;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.Sample
{
    public partial class MainPage : ContentPage
    {
        IBlazorWebView webview;

        public MainPage()
        {
            InitializeComponent();

            //Blazor WebView agnostic contoller logic
            webview = BlazorWebViewFactory.Create();

            //WebView rendering customization on page
            View webviewView = webview.GetView();
            webviewView.VerticalOptions = LayoutOptions.FillAndExpand;
            webviewView.HorizontalOptions = LayoutOptions.FillAndExpand;

            //Manage your native application behavior when an external resource is requested in your Blazor web application
            //Customize your app behavior in BlazorMobile.Sample.Handler.OnBlazorWebViewNavigationHandler.cs file or create your own!
            webview.Navigating += OnBlazorWebViewNavigationHandler.OnBlazorWebViewNavigating;

            webview.LaunchBlazorApp();

            content.Children.Add(webviewView);
        }

        ~MainPage()
        {
            if (webview != null)
            {
                webview.Navigating -= OnBlazorWebViewNavigationHandler.OnBlazorWebViewNavigating;
            }
        }
    }
}
```

And here the default Navigating controller you can implement and/or modify, **OnBlazorWebViewNavigationHandler**:

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

- If you have an **ElectronNET project** for creating a Desktop app with BlazorMobile:
    - In **Startup.cs** from your **MyAppProject.Desktop** project, **app.UseEndpoints** should now look like this:
    ```csharp
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorMobileRequestValidator();
                endpoints.MapFallbackToPage("/server_index");
            });
    ```
    - Still in **Startup.cs**, this...:
    ```csharp
        app.UseBlazorMobileWithElectronNET<App>();
    ```

    - ...Should now look like this:

    ```csharp
        app.UseBlazorMobileWithElectronNET<App>();

        Forms.ConfigureBrowserWindow(new BrowserWindowOptions()
        {
            //Configure the BrowserWindow that will be used for the Blazor application
        });

        //Launch the Blazor app
        Forms.LoadApplication(new App());

        // If your code already started your BlazorWebView.LaunchBlazorApp method, you should retrieve here the Electron main BrowserWindow used to create it.
        // Otherwise, return a null Task value
        var myBrowserWindow = Forms.GetBrowserWindow();
    ```
    - You shold also add some ElectronHostHook code in your project. From a command prompt, go to the root of your **MyAppProject.Desktop** project folder
    and call:

    ```
    electronize add HostHook
    ```
    - Then install the **Microsoft.TypeScript.MSBuild** NuGet package in your **Desktop** project
    - After the package installation, open your Desktop project **ElectronHostHook folder** and replace the content of **index.ts** with:
    ```typescript
    // @ts-ignore
    import * as Electron from "electron";
    import { Connector } from "./connector";

    var blazorMobileRequestValidatorURI: string;

    function blazorMobileRequestValidatorMethod(url: string, referrer: string, mustCancel: Function) : any {

        try {
            let validationURI = blazorMobileRequestValidatorURI + "?uri=" + encodeURIComponent(url) + "&referrer=" + encodeURIComponent(referrer);

            const request = require('electron').net.request({
                method: 'GET',
                url: validationURI
            });

            request.on('response', (response) => {

                if (response.statusCode == "401") {
                    mustCancel(true);
                }
                else {
                    mustCancel(false);
                }
            });

            request.end();

        } catch (e) {
            mustCancel(false);
        }
    }

    export class HookService extends Connector {
        constructor(socket: SocketIO.Socket, public app: Electron.App) {
            super(socket, app);
        }

        onHostReady(): void {
            // execute your own JavaScript Host logic here

            //The current line are required to forward navigating events to the Xamarin.Forms driver,
            //from the main frame and from a subframe
            this.on("add-blazormobile-navigating-behavior", async (serviceURI, done) => {

                //We are a little lazy here, and we are assuming that:
                //- This is called at startup
                //- The first frame to be found is the Blazor app main window
                //We may optimize this in the future

                blazorMobileRequestValidatorURI = serviceURI;
                try {

                    require("electron").webContents.getAllWebContents()[0].session.webRequest.onBeforeRequest((details, cb) =>
                    {
                        if (details.resourceType == "mainFrame" || details.resourceType == "subFrame") {

                            blazorMobileRequestValidatorMethod(details.url, details.referrer, function (cancel) {
                                if (cancel) {
                                    //WARNING: This is a big hack and surely not an expected behavior
                                    //If we return cancel: false, everything is fine
                                    //But if we return cancel: true, the request is canceled,
                                    //but the page navigate to this cancellation

                                    //Instead we are not calling the callback, preventing to do anything
                                    //It may be problematic with a lot of navigation, as we don't know the
                                    //impact internally in Electron
                                    return;
                                }
                                else {
                                    cb({ cancel: false });
                                }
                            });
                        }
                        else {
                            cb({ cancel: false });
                        }
                    });

                    require("electron").webContents.getAllWebContents()[0].on("new-window", (event, url, frameName, disposition, options, additionalFeatures, referrer) => {

                        //We will prevent any new-window
                        //Instead calling our filter for navigation and then opening/showing if allowed by the app
                        event.preventDefault();

                        //Using this current strategy, the event handler may be called twice for verification
                        //on from this window, the other one from the new window if it was allowed, because
                        //it's actually navigating. But it should not be problematic.

                        blazorMobileRequestValidatorMethod(url, referrer.url, function (cancel) {
                            if (!cancel) {
                                //The URL is not cancelled, we should load the window normally
                                const win = new Electron.BrowserWindow({
                                    webContents: options.webContents, // use existing webContents if provided
                                    show: false
                                });

                                win.once('ready-to-show', () => win.show())
                                if (!options.webContents) {
                                    win.loadURL(url) // existing webContents will be navigated automatically
                                }
                                event.newGuest = win;
                            }
                        });
                    });
                } catch (e) {
                    done(e.message);
                }

                done("ok");
            });
        }
    }
    ```
- It is also highly advised to migrate from Xamarin.Forms DependencyService attribute registration to an explicit service registration call.
See [My Xamarin services are not found when interoping in UWP](#my-xamarin-services-are-not-found-when-interoping-in-uwp) for more explanation.

## Authors

- **Guillaume ZAHRA** - [Daddoon](https://github.com/Daddoon) - Software Developer - from Joinville-le-Pont, France. Entrepreneur & founder of 2Bee SASU, working since 10 years with .NET and C#.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
