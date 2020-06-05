
# Migration

- [BlazorMobile 3.2.7-preview4.20210.8 to 3.2.8](#blazormobile-327-preview4202108-to-328)
- [BlazorMobile 3.2.5-preview3.20168.3 to 3.2.7-preview4.20210.8](#blazormobile-325-preview3201683-to-327-preview4202108)
- [BlazorMobile 3.2.4-preview2.20160.5 to 3.2.5-preview3.20168.3](#blazormobile-324-preview2201605-to-325-preview3201683)
- [BlazorMobile 3.2.3-preview2.20160.5 to 3.2.4-preview2.20160.5](#blazormobile-323-preview2201605-to-324-preview2201605)
- [BlazorMobile 3.2.2-preview1.20073.1 to 3.2.3-preview2.20160.5](#blazormobile-322-preview1200731-to-323-preview2201605)
- [BlazorMobile 3.2.0-preview1.20073.1 to 3.2.2-preview1.20073.1](#blazormobile-320-preview1200731-to-322-preview1200731)
- [BlazorMobile 3.1.0-preview3.19555.2 to 3.2.0-preview1.20073.1](#blazormobile-310-preview3195552-to-320-preview1200731)
- [BlazorMobile 3.1.0-preview1.19508.20 to 3.1.0-preview3.19555.2](#blazormobile-310-preview11950820-to-310-preview3195552)
- [BlazorMobile 3.0.12-preview9.19465.2 to 3.1.0-preview1.19508.20](#blazormobile-3012-preview9194652-to-310-preview11950820)
- [BlazorMobile 3.0.11-preview9.19465.2 to 3.0.12-preview9.19465.2](#blazormobile-3011-preview9194652-to-3012-preview9194652)
- [BlazorMobile 3.0.10-preview9.19424.4 to 3.0.11-preview9.19465.2](#blazormobile-3010-preview9194244-to-3011-preview9194652)
- [BlazorMobile 3.0.9-preview8.19405.7 to 3.0.10-preview9.19424.4](#blazormobile-309-preview8194057-to-3010-preview9194244)
- [BlazorMobile 3.0.8-preview8.19405.7 to 3.0.9-preview8.19405.7](#blazormobile-308-preview8194057-to-309-preview8194057)
- [BlazorMobile 3.0.7-preview8.19405.7 to 3.0.8-preview8.19405.7](#blazormobile-307-preview8194057-to-308-preview8194057)
- [BlazorMobile 3.0.6-preview8.19405.7 to 3.0.7-preview8.19405.7](#blazormobile-306-preview8194057-to-307-preview8194057)
- [BlazorMobile 3.0.5-preview8.19405.7 to 3.0.6-preview8.19405.7](#blazormobile-305-preview8194057-to-306-preview8194057)
- [BlazorMobile 3.0.4-preview7.19365.7 to 3.0.5-preview8.19405.7](#blazormobile-304-preview7193657-to-305-preview8194057)
- [BlazorMobile 3.0.3-preview7.19365.7 to 3.0.4-preview7.19365.7](#blazormobile-303-preview7193657-to-304-preview7193657)
- [BlazorMobile 0.8.0 to 3.0.3-preview7.19365.7](#blazormobile-080-to-303-preview7193657)

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
    BlazorMobileService.EnableClientToDeviceRemoteDebugging("192.168.1.118", 8888);

    #endif

    BlazorMobileService.Init(app, (bool success) =>
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

### BlazorMobile 3.0.11-preview9.19465.2 to 3.0.12-preview9.19465.2

This is a minor update, that fix issues on some environments at build time.

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.0.12-preview9.19465.2
```

- Update all your BlazorMobile.* NuGet packages to 3.0.12-preview9.19465.2.

- If you already created a solution with BlazorMobile version **3.0.11-preview9.19465**, there is a missing, not mandatory but recommended, build dependency to add on your project. Right click on **YourProject.AppPackage** project -> **Build dependencies** -> **Project dependencies** -> Check **YourProject.Blazor** project. This will enforce your **AppPackage** project to be in sync with the Blazor project at build time.

- Edit your **YourProject.Android** csproj file and replace theses lines:

```xml
<PackageReference Include="Xamarin.Forms" Version="3.5.0.169047" />
<PackageReference Include="Xamarin.Android.Support.Design" Version="28.0.0.3" />
<PackageReference Include="Xamarin.Android.Support.v7.AppCompat" Version="28.0.0.3" />
<PackageReference Include="Xamarin.Android.Support.v4" Version="28.0.0.3" />
<PackageReference Include="Xamarin.Android.Support.v7.CardView" Version="28.0.0.3" />
<PackageReference Include="Xamarin.Android.Support.v7.MediaRouter" Version="28.0.0.3" />
```

By the new format:

```xml
<PackageReference Include="Xamarin.Forms">
  <Version>3.5.0.169047</Version>
</PackageReference>
<PackageReference Include="Xamarin.Android.Support.Design">
  <Version>28.0.0.3</Version>
</PackageReference>
<PackageReference Include="Xamarin.Android.Support.v7.AppCompat">
  <Version>28.0.0.3</Version>
</PackageReference>
<PackageReference Include="Xamarin.Android.Support.v4">
  <Version>28.0.0.3</Version>
</PackageReference>
<PackageReference Include="Xamarin.Android.Support.v7.CardView">
  <Version>28.0.0.3</Version>
</PackageReference>
<PackageReference Include="Xamarin.Android.Support.v7.MediaRouter">
  <Version>28.0.0.3</Version>
</PackageReference>
```

That's all !

### BlazorMobile 3.0.12-preview9.19465.2 to 3.1.0-preview1.19508.20

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.1.0-preview1.19508.20
```

- [Install .NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) if not already done

- Update all your .NET Core projects (Desktop and Server projects) from .NET Core 3.0 to **.NET Core 3.1**

- Update all your Microsoft.AspNetCore.* NuGet packages to **3.1.0-preview1.19508.20** version

- Update all your BlazorMobile.* NuGet packages to **3.1.0-preview1.19508.20**.

- If you are using ElectronNET.API NuGet package directly on your own Desktop project, update from ElectronNET.API 5.22.14 to **ElectronNET.API 5.30.1**

- Update your ElectronNET.CLI by calling theses commands in a command prompt:

```console
dotnet tool uninstall ElectronNET.CLI -g
dotnet tool install ElectronNET.CLI -g
```

Calling in this order will uninstall the previous version, and then install the latest one.

- For sanity check, delete all **obj** and **bin** folders of your solution manually, and rebuild your solution then.

### BlazorMobile 3.1.0-preview1.19508.20 to 3.1.0-preview3.19555.2

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.1.0-preview3.19555.2
```

- Update all your Microsoft.AspNetCore.* NuGet packages to **3.1.0-preview3.19555.2** version

- Update all your BlazorMobile.* NuGet packages to **3.1.0-preview3.19555.2**.

- In your **index.html** file, remove the reference to **blazor.polyfill.min.js** as it should not be used anymore as it can cause some weird behavior on new versions of Blazor WASM.
If you need to include it in the future for a pure Blazor web app with support for IE11 in server-side mode, ensure that your are loading it only when the browser is IE11. See updated doc of [Blazor.Polyfill](https://github.com/Daddoon/Blazor.Polyfill) if needed.

- You can also delete the **blazor.polyfill.js** file present in the **wwwroot/js** folder of your Blazor app.

- For sanity check, delete all **obj** and **bin** folders of your solution manually, and rebuild your solution then.

### BlazorMobile 3.1.0-preview3.19555.2 to 3.2.0-preview1.20073.1

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.2.0-preview1.20073.1
```

- Update your Blazor projects to **3.2.0-preview1.20073.1**. See [this documentation](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-preview-1-release-now-available/) for upgrading steps.

- Update all your BlazorMobile.* NuGet packages to **3.2.0-preview1.20073.1**.

- Update ElectronNET global tooling by uninstall and reinstalling it.

```console
dotnet tool uninstall ElectronNET.CLI -g
dotnet tool install ElectronNET.CLI -g
```

- If you followed the Blazor project migration for Blazor WebAssembly, this kind of project must not have any **Startup.cs** anymore.
So your code logic should have gone in your **Program.cs** file. The template **Program.cs** file of the Blazor project now look like this,
adapt the code with your custom logic:

```csharp
using BlazorMobile.Common;
using BlazorMobile.Common.Services;
using BlazorMobile.Sample.Blazor.Helpers;
using Microsoft.AspNetCore.Blazor.Hosting;
using System;
using System.Threading.Tasks;

namespace BlazorMobile.Sample.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            #region Services registration

            ServicesHelper.ConfigureCommonServices(builder.Services);

            #endregion

            #region DEBUG

            //Only if you want to test WebAssembly with remote debugging from a dev machine
            BlazorMobileService.EnableClientToDeviceRemoteDebugging("127.0.0.1", 8888);

            #endregion

            BlazorMobileService.Init((bool success) =>
            {
                Console.WriteLine($"Initialization success: {success}");
                Console.WriteLine("Device is: " + BlazorDevice.RuntimePlatform);
            });

            builder.RootComponents.Add<MobileApp>("app");

            await builder.Build().RunAsync();
        }
    }
}
```

- BlazorMobile **Server** (for debugging) and **Desktop** (ElectronNET) projects must have theses additionnal lines in **Startup.cs**...

```csharp
services.AddRazorPages();
```

...in **ConfigureServices** method.

Also this:

```csharp
app.UseClientSideBlazorFiles<BlazorMobile.Sample.Blazor.Program>();
app.UseStaticFiles();
```

...in **Configure** method.

Note that **UseClientSideBlazorFiles** was already present, but now target **Program** instead of **Startup** class.

- Change any references of:

```csharp
services.AddBlazorMobileNativeServices<Startup>();
```

to:

```csharp
services.AddBlazorMobileNativeServices<Program>();
```

### BlazorMobile 3.2.0-preview1.20073.1 to 3.2.2-preview1.20073.1

#### Release note:

- Fix possible **artifacts** folder missing at build time when compiling BlazorMobile Nugets from source code
- Downgraded ElectronNET API to 5.30.1 due to some issues on 7.30.2
- Added WebAssembly engine support on **Desktop** (ElectronNET) project. This is also required if using any **LoadPackage** API of BlazorMobile
- Fix/Add Razor Class Library (RCL) support missing
- Fix possible "Access Denied" error at build time with **BlazorMobile.Build** package.
- Allow loading different BlazorMobile packages during application lifetime, but only one at once.
- Add a package store API in order to store and load external packages, like downloaded from a Stream.

#### Migration guide:

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.2.2-preview1.20073.1
```

- Update all your BlazorMobile.* NuGet packages to **3.2.2-preview1.20073.1**.

- **Downgrade** your ElectronNET global tooling to **5.30.1** as there is some issue while using the latest version actually.

```console
dotnet tool uninstall ElectronNET.CLI -g
dotnet tool install ElectronNET.CLI --version 5.30.1 -g
```

- If you have any reference to the ElectronNET.API NuGet package in your project, try to downgrade it to **5.30.1** too

- If you want to switch the current loaded app, or side-load an app package like for updating your app remotely on ElectronNET,
you will have to run it on the WebAssembly engine instead of Server-Side / .NET Core. The new default template for **Desktop** is referencing both mode for
conveniences, but it's advised that you only keep one mode only when you will going on production, otherwise your app will always be twice is
size.

- In order to be in sync with the new **Desktop** template, here are the things to changes on your ***.Desktop** project:
    - In **appsettings.json**, under **AllowedHosts**, add:
        ```json
        "UseWasmEngine": true
        ```
   - In your project references, add a reference to the **AppPackage** project.
   - In your **Startup.cs** file change this:
     
    ```csharp
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    ```
    to

    ```csharp
    private bool useWASM = false;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        useWASM = (bool)Configuration.GetValue<bool>("UseWasmEngine");
    }
    ```

    Wrap all your ASP.NET Core specific code in a **if (!useWASM)** condition in **Configure** method, ending just before **app.UseBlazorMobileWithElectronNET** method.
    It could look like:

    ```csharp
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!useWASM)
        {
            /* Your ASP.NET / Blazor Server-Side init code */
        }

        app.UseBlazorMobileWithElectronNET<App>(useWASM);
    }
    ```

    After **UseBlazorMobileWithElectronNET**, add this code:

    ```csharp
    //Theses line must be registered after 'UseBlazorMobileWithElectronNET' as it internally call Xamarin.Forms.Init()
    if (useWASM)
    {
        BlazorWebViewService.Init();

        //Register our Blazor app package
        WebApplicationFactory.RegisterAppStreamResolver(AppPackageHelper.ResolveAppPackageStream);
    }
    ```

    Next part is unchanged! In doubt, here is the full current default **Startup.cs** for **Desktop** from template code:

    ```csharp
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System.Net.Http;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.ResponseCompression;
    using BlazorMobile.Common.Services;
    using BlazorMobile.Common;
    using BlazorMobile.Sample.Blazor.Helpers;
    using System.Threading.Tasks;
    using ElectronNET.API;
    using BlazorMobile.Sample.Blazor;
    using Xamarin.Forms;
    using ElectronNET.API.Entities;
    using BlazorMobile.ElectronNET.Services;
    using BlazorMobile.Services;
    using BlazorMobile.Sample.AppPackage;

    namespace BlazorMobile.Sample.Desktop
    {
        public class Startup
        {
            private bool useWASM = false;

            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
                useWASM = (bool)Configuration.GetValue<bool>("UseWasmEngine");
            }

            public IConfiguration Configuration { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvc().AddNewtonsoftJson();
                services.AddRazorPages();
                services.AddServerSideBlazor();
                services.AddResponseCompression(opts =>
                {
                    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "application/octet-stream" });
                });

                // Server Side Blazor doesn't register HttpClient by default
                if (!services.Any(x => x.ServiceType == typeof(HttpClient)))
                {
                    // Setup HttpClient for server side in a client side compatible fashion
                    services.AddScoped<HttpClient>(s =>
                    {
                        // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
                        var uriHelper = s.GetRequiredService<NavigationManager>();
                        return new HttpClient
                        {
                            BaseAddress = new Uri(uriHelper.BaseUri)
                        };
                    });
                }

                ServicesHelper.ConfigureCommonServices(services);
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (!useWASM)
                {
                    app.UseResponseCompression();

                    if (env.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                        app.UseBlazorDebugging();
                    }
                    else
                    {
                        app.UseExceptionHandler("/Home/Error");
                    }

                    app.UseClientSideBlazorFiles<BlazorMobile.Sample.Blazor.Program>();

                    app.UseStaticFiles();
                    app.UseRouting();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapBlazorHub();
                        endpoints.MapDefaultControllerRoute();
                        endpoints.MapBlazorMobileRequestValidator();
                        endpoints.MapFallbackToPage("/server_index");
                    });

                    //Initialize Blazor app from .NET Core / Server-side
                    BlazorMobileService.Init((bool success) =>
                    {
                        Console.WriteLine($"Initialization success: {success}");
                        Console.WriteLine("Device is: " + BlazorDevice.RuntimePlatform);
                    });
                }

                app.UseBlazorMobileWithElectronNET<App>(useWASM);

                //Theses line must be registered after 'UseBlazorMobileWithElectronNET' as it internally call Xamarin.Forms.Init()
                if (useWASM)
                {
                    BlazorWebViewService.Init();

                    //Register our Blazor app package
                    WebApplicationFactory.RegisterAppStreamResolver(AppPackageHelper.ResolveAppPackageStream);
                }

                Forms.ConfigureBrowserWindow(new BrowserWindowOptions()
                {
                    //Configure the BrowserWindow that will be used for the Blazor application
                });

                //Launch the Blazor app
                Forms.LoadApplication(new App());

                // If your code already started your BlazorWebView.LaunchBlazorApp method, you should retrieve here the Electron main BrowserWindow used to create it.
                // Otherwise, return a null Task value
                var myBrowserWindow = Forms.GetBrowserWindow();
            }
        }
    }
    ```

### BlazorMobile 3.2.2-preview1.20073.1 to 3.2.3-preview2.20160.5

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.2.3-preview2.20160.5
```

- Update your Blazor WebAssembly project to **3.2.0-preview2.20160.5**. See this [Microsoft migration guide to Blazor WebAssembly 3.2.0 Preview 2](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-preview-2-release-now-available/)

- Update all your BlazorMobile.* NuGet packages to **3.2.3-preview2.20160.5**.

### BlazorMobile 3.2.3-preview2.20160.5 to 3.2.4-preview2.20160.5

#### Release notes:

- Add **select** tag behavior support missing on Android
- Add **input** type **file** behavior support missing on Android

#### Migration guide:

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.2.4-preview2.20160.5
```

- Update all your BlazorMobile.* NuGet packages to **3.2.4-preview2.20160.5**.

- In your **MainActivity.cs** file in your Android project, make your **MainActivity** class inheriting from **BlazorMobileFormsAppCompatActivity** instead of **FormsAppCompatActivity**.
  
  Before:

  ```csharp
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
  ```

  After:

  ```csharp
    public class MainActivity : global::BlazorMobile.Droid.Platform.BlazorMobileFormsAppCompatActivity
    {
  ```

- If you have your own overrides on **OnActivityResult** or **OnCreate** on your **MainActivity** class, be sure to not forget to call the base implementation.

### BlazorMobile 3.2.4-preview2.20160.5 to 3.2.5-preview3.20168.3

#### Release notes:

- Fixed a potentially race condition preventing BlazorMobile to not boot properly on some apps configurations
- Fix Desktop (ElectronNET) app application not booting

#### Migration guide:

- Update your Blazor project version to Blazor 3.2.0-preview3.20168.3 by follow this [Microsoft migration](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-preview-3-release-now-available/)

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.2.5-preview3.20168.3
```

- Update all your BlazorMobile.* NuGet packages to **3.2.5-preview3.20168.3**.

- Replace all **BlazorMobileService.Init** calls to **BlazorMobileService.OnBlazorMobileLoaded**.
  You should go from this:

  ```csharp
    BlazorMobileService.Init((bool success) =>
    {
        Console.WriteLine($"Initialization success: {success}");
        Console.WriteLine("Device is: " + BlazorDevice.RuntimePlatform);
    });
  ```

  to this:

  ```csharp
    BlazorMobileService.OnBlazorMobileLoaded += (object source, BlazorMobileOnFinishEventArgs eventArgs) =>
    {
        Console.WriteLine($"Initialization success: {eventArgs.Success}");
        Console.WriteLine("Device is: " + BlazorDevice.RuntimePlatform);
    };
  ```

- Open your **MobileApp.cs** file. You should replace this:

  ```csharp
    using BlazorMobile.Common.Components;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Rendering;
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

  to this:

  ```csharp
    using BlazorMobile.Common.Components;
    using BlazorMobile.Common.Services;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Rendering;
    using Microsoft.AspNetCore.Components.RenderTree;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    namespace BlazorMobile.Sample.Blazor
    {
        public class MobileApp : App
        {
            public MobileApp() : base()
            {
                //In the first sequence we must only load BlazorMobile component
                //When BlazorMobile will be ready, we will render the app
                BlazorMobileService.OnBlazorMobileLoaded += BlazorMobileService_OnBlazorMobileLoaded;
            }

            private void BlazorMobileService_OnBlazorMobileLoaded(object source, BlazorMobileOnFinishEventArgs args)
            {
                //InvokeAsync is mainly needed for .NET Core implementation that need the renderer context
                InvokeAsync(() =>
                {
                    //BlazorMobile is ready. We should call StateHasChanged method in order to call BuildRenderTree again.
                    //This time, it should load your app with base.BuildRenderTree() method call.
                    BlazorMobileService.HideElementById("placeholder");
                    StateHasChanged();
                }
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, nameof(BlazorMobileComponent));
                builder.OpenComponent(1, typeof(BlazorMobileComponent));
                builder.CloseComponent();
                builder.CloseElement();

                if (BlazorMobileService.IsBlazorMobileLoaded)
                {
                    base.BuildRenderTree(builder);
                }
            }
        }
    }
  ```

- Open **index.html** and move the loading placeholder content to a sibling tag with the **id** mentionned in the previous code in **BlazorMobileService.HideElementById("placeholder")**.
  So if you have kept the default configuration from templates, this should go from this:

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
        <script type="text/javascript" src="_framework/blazor.webassembly.js"></script>
    </body> 
    </html>
  ```

  to this:

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
        <app></app>
        <div id="placeholder">Loading...</div>
        <script type="text/javascript" src="_framework/blazor.webassembly.js"></script>
    </body> 
    </html>
  ```

  This final step was not mandatory, but as Blazor by design does not keep inner HTML content if a component start to be rendered, this change allow to keep the loading placeholder during BlazorMobile and BlazorMobileComponent initialization.
  **BlazorMobileService.HideElementById("placeholder");** method call in **MobileApp.cs** hide the placeholder when everything is ready.

- Add the **BlazorMobile.Build.ElectronNET** NuGet package to the **.Desktop** project. This workaround / fix a publishing issue since some recent updates, preventing Desktop project to boot correctly.

### BlazorMobile 3.2.5-preview3.20168.3 to 3.2.7-preview4.20210.8

#### Migration guide:

- Update your Blazor project version to Blazor 3.2.0-preview4.20210.8 by follow this [Microsoft migration](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-preview-4-release-now-available/)

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.2.7-preview4.20210.8
```

- Update all your BlazorMobile.* NuGet packages to **3.2.7-preview4.20210.8**.

- Open the **index.html** file of your Blazor project and add the following line before the **blazor.webassembly.js** line:

```html
    <script type="text/javascript" src="_content/BlazorMobile.Web/blazormobile.js"></script>
```

  So your **index.html** may look like this:

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
    <app></app>
    <div id="placeholder">Loading...</div>
    <script type="text/javascript" src="_content/BlazorMobile.Web/blazormobile.js"></script>
    <script type="text/javascript" src="_framework/blazor.webassembly.js"></script>
</body> 
</html>
```

### BlazorMobile 3.2.7-preview4.20210.8 to 3.2.8

#### Release notes:

- Fix **Could not load file or assembly 'BlazorMobile.Common, Version=3.2.7.0, Culture=neutral, PublicKeyToken=null'. The located assembly's manifest definition does not match the assembly reference. (0x80131040)'** error on .NET Core / Server's project at startup.
- Updated Blazor version to **3.2.0**

#### Migration guide:

- Update your Blazor project version to Blazor 3.2.0 by follow theses guides, if you upgrade from 3.2.4-preview4:
  - [Blazor 3.2.0-preview5](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-preview-5-release-now-available/)
  - [Blazor 3.2.0-RC](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-release-candidate-now-available/)
  - [Blazor 3.2.0](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-now-available/)

- Update your installed BlazorMobile.Templates to this version by calling:

```console
dotnet new -i BlazorMobile.Templates::3.2.8
```

- Update all your BlazorMobile.* NuGet packages to **3.2.8**.