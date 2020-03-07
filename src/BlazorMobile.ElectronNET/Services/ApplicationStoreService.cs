using BlazorMobile.Common.Helpers;
using BlazorMobile.Helper;
using BlazorMobile.Interop;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.ElectronNET.Services
{
    public class ApplicationStoreService : IApplicationStoreService
    {
        public ApplicationStoreService()
        {
            var service = Forms.GetBlazorMobilePlatformServices();
            var storage = (ElectronIsolatedStorage)service.GetUserStoreForApplication();
            string userDataFolder = storage.GetUserDataDirectory();

            ApplicationStoreHelper.SetPackageFolder(Path.Combine(userDataFolder, Consts.Constants.PackageStoreDirectoryName));
        }

        public bool AddPackage(string name, Stream content)
        {
            return ApplicationStoreHelper.AddPackage(name, content);
        }

        public Task<bool> AddPackageAsync(string name, Stream content)
        {
            return ApplicationStoreHelper.AddPackageAsync(name, content);
        }

        public Func<Stream> GetPackageStreamResolver(string name)
        {
            return ApplicationStoreHelper.GetPackageStreamResolver(name);
        }

        public IEnumerable<string> ListPackages()
        {
            return ApplicationStoreHelper.ListPackages();
        }

        public bool RemovePackage(string name)
        {
            return ApplicationStoreHelper.RemovePackage(name);
        }
    }
}
