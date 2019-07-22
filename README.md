# BlazorMobile
Create full C# driven hybrid-apps for iOS & Android !

**BlazorMobile** - is a Nuget package for embedding a Blazor web application as a standalone mobile application, hosted in Xamarin.

## Platform requirements
  
- **Android:** Android 5.0 or greater
- **iOS:** iOS 12 or greater
- **Blazor:** 3.0.0-preview6.19307.2

**The current version has been developed and tested on Blazor 3.0.0-preview6.19307.2**

## Summary

- [Getting started from sample](#getting-started-from-sample)
- [Detecting Runtime Platform](#detecting-runtime-platform)
- [Communication between Blazor & Xamarin.Forms](#communication-between-blazorxamarinforms)
- [Device remote debugging & Debugging from NET Core 3.0](#device-remote-debugging--debugging-from-net-core-30)
- [Migration: Migrating from BlazorMobile 0.8.0/3.0.0-preview6 to 3.0.1-preview6.19307.2](#test)

## Getting started from sample

The easiest way in order to start is to [download the sample projects](https://github.com/Daddoon/BlazorMobile/releases/download/3.0.1-preview6.19307.2/BlazorMobile.Samples.zip). Unzip, and open the solution, and you are good to go.

If you want to install from scratch, read below.

## Communication between Blazor & Xamarin.Forms

In order to communicate from Blazor to Xamarin you need to do some few steps, as JIT is disabled on AOT environment like Blazor.
Here is a simple example to Display a Xamarin.Forms alert from Blazor.

**In your shared project for Blazor & Xamarin**, create an interface in an Interfaces folder, and add the ProxyInterface attribute on it. Assuming a **IXamarinBridge** interface class, present on the **BlazorMobile.Sample.Common project**.

Your file could look like this:

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

**In your Xamarin shared application project**, implement the Device implementation, also referenced as a DependencyService (notice the attribute here). Assuming adding it like in **BlazorMobile.Sample project**.
	
Your implementation may look like this. Here a kind of useless example:

```csharp
using BlazorMobile.Sample.Common.Interfaces;
using BlazorMobile.Sample.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(XamarinBridge))]
namespace BlazorMobile.Sample.Services
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

**In your Blazor project**, implement the proxy service class implementation.

_**Note:** We must help the call the proxy by ourself, as the Blazor WASM implementation does not support any kind of dynamic dispatcher, as **System.Reflection.Emit is not available in this context**. Just keep using the same logic as in the example below._

For our example it look like this in our **BlazorMobile.Sample.Blazor** project:

```csharp
using BlazorMobile.Common.Services;
using BlazorMobile.Sample.Common.Interfaces;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorMobile.Sample.Blazor.Services
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

The **MethodDispatcher** class, that will prepare every callback for you, calling the right interface and parameters types on the Xamarin side, if you wrote everything right.

Because of the lack of JIT, you have to give yourself some parameters. Take a look at the different implementations of MethodDispatcher methods, in order to accord everything to your context, like if your using Task (Async calls) or not, if you expect a return value, generic types...

There is actually some syntactic sugar method calls in order to just mimic what you are expecting, by just recognizing the same kind of signature, if using generic parameters etc. You may take a look at the [MethodDispatcher file](https://github.com/Daddoon/BlazorMobile/blob/master/src/BlazorMobile.Common/Services/MethodDispatcher.cs) if you want to see the available methods overload.

If you want that the caller and receiver method are actually the same method signature on the 2 ends (Blazor & Xamarin), you can safely use MethodBase.GetCurrentMethod() everytime for the MethodInfo parameter, like in our example.

## Detecting Runtime Platform

If your Blazor application is ready by taking the samples or followed the installation from scratch, you should have the **BlazorService.Init()** already called in the **Startup.cs** file.
Then you only need to call:

```csharp
Device.RuntimePlatform
```

...In order to retrieve the current device runtime platform.

Note that the **BlazorService.Init()** has an **onFinish** optional callback delegate. Every call to **Device.RuntimePlatform** before the onFinish delegate call will return **Device.Unkown** instead of the detected platform.

### Test your interop with Xamarin in Blazor

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
            app.AddComponent<MobileApp>("app");
        }
    }
```

In our sample project **BlazorMobile.Sample.Blazor**, we moved services initialization in **ServicesHelper.ConfigureCommonServices** static method.

Then in one of your desired **razor** page (or plain **C# ComponentBase**), juste add...
```csharp
@inject IXamarinBridge XamarinBridge
```

...on the top of your razor file, then call your method in your desired callback, like:

```csharp
var result = await XamarinBridge.DisplayAlert("Platform identity", $"Current platform is {Device.RuntimePlatform}", "Great!");
```

If using this example the sample project, clicking on the **Alert Me** button on the **Counter page** should show you the **native device alert**, with the given parameters, and showing you the **current detected device runtime platform**, like iOS or Android.

## Device remote debugging & Debugging from NET Core 3.0

Even if there is now some debug functionalities in the Blazor WASM version in Chrome, it is pretty limited compared to the pure server-side debugging with NET Core 3.0.

A small server-side Blazor application sample has been added in order to test and debug your code from it. See **BlazorMobile.Sample.Blazor.Server** project.
You don't have to code anything in it, as it will use all the code logic you have done with the **BlazorMobile.Sample.Blazor** project (the WASM one).

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

#### Deploy & Launch mobile application, debug from PC

Then, you just need to deploy your application to your phone, and launch it in order to allow external source to connect to it.
You may just launch it on the device, and only debug Blazor from your PC, or you may also launch it with the Xamarin debugger, in order to test Xamarin code during Blazor session.

If you want to debug both Blazor side and Xamarin side, i suggest to open two Visual Studio instances, one for launching debug on the Xamarin project on your device, and the other instance for debugging the Blazor application.
The Blazor application will be launched from your PC, and it will try to connect to the remote application instance.

Values from Xamarin context will be returned, and your code will behave as launched within the device.

**NOTE:** You **need** to add the **?mode=server** URI parameters on your PC when debugging your Blazor app in order to debug from the NET Core version.
Default **BlazorMobile.Sample.Blazor.Server** project should listen on http://localhost:5080/.

When the server console will show up during your debugging session, you need to open a tab in your favorite browser and browse http://localhost:5080/?mode=server url, in order to connect and debug your Blazor NET Core application.

If you omit the mode=server argument, the Blazor application will be launched as the WASM one.

## Migration

### Migrating from BlazorMobile 0.8.0/3.0.0-preview6 to 3.0.1-preview6.19307.2

Coming very soon

## Authors

* **Guillaume ZAHRA** - [Daddoon](https://github.com/Daddoon)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
