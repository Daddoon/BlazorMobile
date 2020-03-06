using BlazorMobile.Common;
using BlazorMobile.InteropApp.Common.Interfaces;
using BlazorMobile.InteropApp.Services;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.InteropApp.Services
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

        public Task CallFaultyTask()
        {
            throw new InvalidOperationException("This is an expected exception");
        }

        public Task TriggerPostMessageTest()
        {
            MainPage.webview.PostMessage("PostMessageTest", "My posted string");
            return Task.CompletedTask;
        }

        public Task TriggerPostMessageTestBool()
        {
            MainPage.webview.PostMessage("PostMessageTest", true);
            return Task.CompletedTask;
        }

        public Task TriggerJSInvokableTest()
        {
            MainPage.webview.CallJSInvokableMethod("BlazorMobile.InteropBlazorApp", "InvokableMethodTest", "My invoked string");
            return Task.CompletedTask;
        }

        public Task SwitchToAnotherAppPackage()
        {
            var assemblyService = DependencyService.Get<IAssemblyService>();

            //For ApplicationStore test, trying to Add alternate project package to the Store
            //Then we load it and wait 5 seconds
            //Then we load the regular app
            //Then we remove the alternate project from disk

            string regularApp = "Package.BlazorMobile.InteropBlazorApp.zip";
            string alternateApp = "Package.BlazorMobile.InteropBlazorApp.AnotherApp.zip";

            Assembly packageAssembly = assemblyService.GetAppPackageAssembly();
            Stream fakeHttpPackage = packageAssembly.GetManifestResourceStream($"{packageAssembly.GetName().Name}.{alternateApp}");

            string packageStoreName = "alternate_package.zip";

            bool addPackageSuccess = WebApplicationFactory.AddPackage(packageStoreName, fakeHttpPackage);

            //For debug inspection
            var packageResults = WebApplicationFactory.ListPackages().ToList();

            bool loadPackageSuccess = WebApplicationFactory.LoadPackage(packageStoreName);

            Task.Run(async () =>
            {
                await Task.Delay(5000);
                Device.BeginInvokeOnMainThread(() =>
                {
                    //Reload regular package
                    WebApplicationFactory.LoadPackageFromAssembly(assemblyService.GetAppPackageAssembly(), regularApp);
                    WebApplicationFactory.RemovePackage(packageStoreName);

                    //alternate_package.zip should not be present
                    var packageResultsAfterRemove = WebApplicationFactory.ListPackages().ToList();
                });
            });

            return Task.CompletedTask;
        }
    }
}
