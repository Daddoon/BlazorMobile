# Blazor.Xamarin
A Nuget package for launching Blazor application as standalone application on Xamarin

# INSTALLATION

**1. Create your Xamarin.Forms application project in Visual Studio**

The ideal scenario, as the given templates in this repository, is to create a Cross-plateform Xamarin project template.
You then should have your solution this type of configuration:

- YourApp (.NetStandard 2.0)
- YourApp.Droid (MonoDroid)
- YourApp.iOS (Xamarin.iOS)
- YourApp.UWP (UWP)

**YourApp** project will be used as the Blazor app container, it's not mandatory but highly recommended.

**2. ZIP your Blazor app project ! Our plugin need to read a Blazor app zipped in an archive for maintenability convenience.**

As you surely want to always have you Blazor app in sync in your mobile standalone app, you may want to automate your ZIP archive content.
The Blazor example template use this command at PostBuild event:

```
rm $(ProjectDir)\BuildTools\Mobile\bin\app.zip >nul 2>&1
$(ProjectDir)\BuildTools\7za.exe a $(ProjectDir)\BuildTools\Mobile\bin\app.zip $(ProjectDir)wwwroot\* -mx1 -tzip
$(ProjectDir)\BuildTools\7za.exe a $(ProjectDir)\BuildTools\Mobile\bin\app.zip $(ProjectDir)$(OutputPath)dist\* -mx1 -tzip
```

Of course adapt the path to your development environement. If use this method, notice to respect the current order, first **wwwroot**, then **dist**, as the dist folder contain also an index.html file, but processed by Blazor tooling: This is the right one to use, not the one you see available in your solution when coding.

**NOTE:** There could be some issue with old browsers that doesn't support some ECMA script standards. You may want to fill the gap in your Blazor project by adding some polyfills. If so, take a look at my [Blazor.Polyfill](https://github.com/Daddoon/Blazor.Polyfill) repository.

**3. Add your Blazor ZIP file as link in YourApp project**

On YourApp project, add your generated ZIP from the Blazor project, as a "link" => Right click on the project => Add existing file => Browse to your file => Click on the little arrow => Then click on **Add as link**

**4. Set your linked file as Embedded Resource**

Do right click on your newly added as link file in YourApp project, and click **Properties**
Then check that the **Build Action property** is on **Embedded Resource**

**5. Add Daddoon.Blazor.Xamarin NuGet package**

Add **Daddoon.Blazor.Xamarin** NuGet package on the following projects:

- YourApp
- YourApp.Droid

The package is available on the nuget.org feed, but you can also download the file manually [in the release page](https://github.com/Daddoon/Blazor.Xamarin/releases)

**6. Platform specific configuration**

You have **nothing to do** for **UWP** and **iOS**

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

**7. Shared/Common project configuration**



# COMMUNICATION BETWEEN BLAZOR/XAMARIN.FORMS

**TODO**: Not yet, but very soon!

# DISCLAIMER

This project is not affiliated with the Blazor project.
