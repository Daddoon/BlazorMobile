using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BlazorMobile.Helper;
using BlazorMobile.Interop;

namespace BlazorMobile.Droid.Services
{
    public class ApplicationStoreService : IApplicationStoreService
    {
        public ApplicationStoreService()
        {
            string internalStorage = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            ApplicationStoreHelper.SetPackageFolder(Path.Combine(internalStorage, Consts.Constants.PackageStoreDirectoryName));
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