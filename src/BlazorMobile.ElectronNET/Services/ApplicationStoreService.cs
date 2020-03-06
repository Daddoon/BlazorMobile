using BlazorMobile.Common.Helpers;
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
        private string _cachedPackageFolder = null;

        private string GetPackagesFolder()
        {
            if (string.IsNullOrEmpty(_cachedPackageFolder))
            {
                var service = Forms.GetBlazorMobilePlatformServices();
                var storage = (ElectronIsolatedStorage)service.GetUserStoreForApplication();
                string userDataFolder = storage.GetUserDataDirectory();

                _cachedPackageFolder = Path.Combine(userDataFolder, Consts.Constants.PackageStoreDirectoryName);
            }

            try
            {
                if (!Directory.Exists(_cachedPackageFolder))
                {
                    Directory.CreateDirectory(_cachedPackageFolder);
                }
            }
            catch (Exception ex)
            {
                //Bubble up in this case
                throw;
            }

            return _cachedPackageFolder;
        }

        private bool FileExist(string fileName)
        {
            string packageFolder = GetPackagesFolder();
            return File.Exists(Path.Combine(packageFolder, fileName));
        }

        public bool AddPackage(string name, Stream content)
        {
            if (content == null)
            {
                throw new NullReferenceException($"{nameof(content)} cannot be null");
            }

            if (FileExist(name))
            {
                ConsoleHelper.WriteError($"{nameof(name)} already exist on device");
                return false;
            }

            string packageFolder = GetPackagesFolder();
            using (Stream fileStream = File.Create(Path.Combine(packageFolder, name)))
            {
                content.CopyTo(fileStream);
                content.Flush();
            }

            return true;
        }

        public Task<bool> AddPackageAsync(string name, Stream content)
        {
            return Task.FromResult(AddPackage(name, content));
        }

        public Func<Stream> GetPackageStreamResolver(string name)
        {
            string packageFile = ListPackagesStorageFile().FirstOrDefault(p => Path.GetFileName(p) == name);
            if (packageFile == null)
            {
                return null;
            }

            Stream package = File.OpenRead(packageFile);

            return () => package;
        }

        private IEnumerable<string> ListPackagesStorageFile()
        {
            string packagesFolder = GetPackagesFolder();
            IEnumerable<string> files = new List<string>();

            try
            {
                files = Directory.GetFiles(packagesFolder, "*", SearchOption.TopDirectoryOnly);
            }
            catch (Exception)
            {
            }

            return files;
        }

        public IEnumerable<string> ListPackages()
        {
            return ListPackagesStorageFile().Select(Path.GetFileName);
        }

        public bool RemovePackage(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new NullReferenceException($"{nameof(name)} cannot be null");
            }

            string loadedPackage = WebApplicationFactory.GetCurrentLoadedApplicationStorePackageName();

            if (!string.IsNullOrEmpty(loadedPackage) && name == loadedPackage)
            {
                ConsoleHelper.WriteError($"Cannot delete package '{name}': package is still opened");
                return false;
            }

            string currentPackage = ListPackagesStorageFile().FirstOrDefault(p => Path.GetFileName(p) == name);
            if (currentPackage == null)
            {
                ConsoleHelper.WriteError("Package '{name}' not found");
                return false;
            }

            try
            {
                File.Delete(currentPackage);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
                return false;
            }

            return true;
        }
    }
}
