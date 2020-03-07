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

namespace BlazorMobile.iOS.Services
{
    public class ApplicationStoreService : IApplicationStoreService
    {
        public ApplicationStoreService()
        {
            string libraryCache = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            ApplicationStoreHelper.SetPackageFolder(Path.Combine(libraryCache, Consts.Constants.PackageStoreDirectoryName));
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
