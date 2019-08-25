using BlazorMobile.Common;
using BlazorMobile.Components;
using BlazorMobile.Services;

namespace BlazorMobile.Sample
{
	public partial class App : BlazorApplication
    {
        public const string BlazorAppPackageName = "BlazorMobile.Sample.Blazor.zip";

        public App()
        {
            InitializeComponent();

            //We do not need to configure any embedded HTTP server from here with Electron as we are already on ASP.NET Core
            //We do not need to set any package to load, nor loading any browser as it's already managed by Electron
            if (BlazorDevice.IsElectronNET())
            {
                return;
            }

#if DEBUG
            WebApplicationFactory.EnableDebugFeatures();
#endif
            WebApplicationFactory.SetHttpPort(8888);

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

            MainPage = new MainPage();
        }
    }
}
