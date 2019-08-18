# BlazorMobile[<img src="logo_blazormobile_256x256.png?raw=true" align="right" width="200">]() 

Create full C# driven hybrid-apps for iOS, Android & UWP !

**BlazorMobile** - is a set of Nuget packages & project templates for embedding a Blazor web application as a standalone mobile application, hosted in Xamarin.

## Platform requirements
  
- **Android:** Android 4.4 or greater
- **iOS:** iOS 12.0 or greater
- **UWP:** Build 16299 or greater
- **Blazor:** 3.0.0-preview8.19405.7

## Summary

- [Difference between BlazorMobile & Progressive Web Apps (PWA)](#difference-between-blazormobile--progressive-web-apps-pwa)
- [Getting started from sample](#getting-started-from-sample)
- [Linking your Blazor app to your Xamarin project](#linking-your-blazor-app-to-your-xamarin-project)
- [Detecting Runtime Platform](#detecting-runtime-platform)
- [Communication between Blazor & Xamarin.Forms](#communication-between-blazor--xamarinforms)
- [Device remote debugging & Debugging from NET Core 3.0](#device-remote-debugging--debugging-from-net-core-30)

## Troubleshoot

- [Cannot connect to a remote webserver on UWP](#cannot-connect-to-a-remote-webserver-on-uwp)

## Migration

- [BlazorMobile 0.8.0 to 3.0.3-preview7.19365.7](#blazormobile-080-to-303-preview7193657)
- [BlazorMobile 3.0.3-preview7.19365.7 to 3.0.4-preview7.19365.7](#blazormobile-303-preview7193657-to-304-preview7193657)
- [BlazorMobile 3.0.4-preview7.19365.7 to 3.0.5-preview8.19405.7](#blazormobile-304-preview7193657-to-305-preview8194057)

## Difference between BlazorMobile & Progressive Web Apps (PWA)

Both creating an application as PWA or using BlazorMobile can be an option with Blazor

The main differences / advantages of BlazorMobile are:

- Access to native

- Access from Web to native both in C#

- More control about your application behaviors, depending your needs and complexity, some type of integration may be difficult with PWA. Still i think the majority of things can be done with PWA only.

- You can support old versions of Android where WebAssembly was even not present. Actually because the WebView component used in the plugin is the excellent Mozilla GeckoView instead, so giving you some consistency accross Android devices. On the other side, PWA will never work on older devices, because of lack of PWA support, or because the browser implementation of the system does not have any support of WebAssembly, required by Blazor.

- If you are good at designing your application, you can even make your application PWA and BlazorMobile compatible, as you can work intensively with DependencyInjection for services, and so, have multiple implementations of your app services in one or another use case !

## Getting started from sample

The easiest way in order to start is to [download the sample projects](https://github.com/Daddoon/BlazorMobile/releases/download/3.0.5-preview8.19405.7/BlazorMobile.Samples.zip). Unzip, and open the solution, and you are good to go.

## Linking your Blazor app to your Xamarin project

In order to ship your Blazor application within your Xamarin apps, you need to pack it and make it available to Xamarin.

Your Blazor app will be automatically packaged thanks to the **BlazorMobile.Build** NuGet package, that must be installed on your Blazor web application project. The package location will be written in the build output after the Blazor build mecanism.

Here are the steps in order to link it in Xamarin:

- Add your package **as a link** in your Xamarin.Forms shared project, from the Blazor web app bin directory.

- Set the property of your package file as an **Embedded Resource** from Visual Studio.

- **Optional**: Add a project dependency on your Xamarin.Forms shared project, and check your Blazor web application as a dependency. **This way we will be assured that even if there is no direct reference between the shared project and the blazor web application assembly, the blazor project and our zip are always updated before building our mobile application project**.

- Set the path to your package in your Xamarin.Forms shared project. In the **App.xaml.cs** file, set the path in your **RegisterAppStreamResolver** delegate.

As seen on the **BlazorMobile.Sample** project, assuming a file linked as in a folder called **Package**, we would have a code like this:

```csharp
namespace BlazorMobile.Sample
{
	public partial class App : Application
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

If you want to debug both Blazor side and Xamarin side, i suggest to open two Visual Studio instances, one for launching debug on the Xamarin project on your device, and the other instance for debugging the Blazor application.
The Blazor application will be launched from your PC, and it will try to connect to the remote application instance.

Values from Xamarin context will be returned, and your code will behave as launched within the device.

**NOTE:** You **need** to add the **?mode=server** URI parameters on your PC when debugging your Blazor app in order to debug from the NET Core version.
Default **BlazorMobile.Sample.Blazor.Server** project should listen on http://localhost:5080/.

When the server console will show up during your debugging session, you need to open a tab in your favorite browser and browse http://localhost:5080/?mode=server url, in order to connect and debug your Blazor NET Core application.

If you omit the mode=server argument, the Blazor application will be launched as the WASM one.

Of course you can change this behavior by your own logic, just take a look at **index.html** on how the Blazor javascript file is loaded.

## Troubleshoot

### Cannot connect to a remote webserver on UWP

There is some behaviors that are specifics to UWP:

- You cannot connect to a local webserver / socket endpoint, out of process, on the same machine. This mean that if your testing development about webservices from IIS, Kestrel or other, UWP will be unable to connect to them. The server must be present on an other machine.


- If you are doing any web requests on with HTTPS, UWP will block them if the certificate is self-signed or not trusted. You may override this behavior if your are doing your requests from the native side instead as you may have more control about web requests behavior, but this may be less ideal from a Design point of view.

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

## Authors

* **Guillaume ZAHRA** - [Daddoon](https://github.com/Daddoon)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
