using BlazorMobile.Common.Helpers;
using BlazorMobile.Interop;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Xamarin.Forms.Internals;

namespace BlazorMobile.UWP.Services
{
    public class ApplicationStoreService : IApplicationStoreService
    {
        private bool FileExist(string fileName)
        {
            StorageFolder packageFolder = GetPackagesFolder();

            var item = packageFolder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            return item != null;
        }

        public async Task<bool> AddPackageAsync(string name, Stream content)
        {
            if (content == null)
            {
                throw new NullReferenceException($"{nameof(content)} cannot be null");
            }

            if (FileExist(name))
            {
                throw new InvalidOperationException($"{nameof(name)} already exist on device");
            }

            StorageFolder packageFolder = GetPackagesFolder();
            using (Stream fileStream = await packageFolder.OpenStreamForWriteAsync(name, CreationCollisionOption.ReplaceExisting))
            {
                content.CopyTo(fileStream);
                content.Flush();
            }

            return true;
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

            StorageFolder packageFolder = GetPackagesFolder();
            using (Stream fileStream = packageFolder.OpenStreamForWriteAsync(name, CreationCollisionOption.ReplaceExisting).ConfigureAwait(false).GetAwaiter().GetResult())
            {
                content.CopyTo(fileStream);
                content.Flush();
            }

            return true;
        }

        private StorageFolder GetPackagesFolder()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalCacheFolder;
            StorageFolder packagesFolder;

            try
            {
                packagesFolder = storageFolder.CreateFolderAsync(Consts.Constants.PackageStoreDirectoryName, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                //Bubble up in this case
                throw;
            }

            return packagesFolder;
        }

        public Func<Stream> GetPackageStreamResolver(string name)
        {
            StorageFile packageFile = ListPackagesStorageFile().FirstOrDefault(p => p.Name == name);
            if (packageFile == null)
            {
                return null;
            }

            Stream package = packageFile.OpenStreamForReadAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            return () => package;
        }

        private IEnumerable<StorageFile> ListPackagesStorageFile()
        {
            StorageFolder packagesFolder = GetPackagesFolder();
            IReadOnlyList<StorageFile> files = new StorageFile[0];

            try
            {
                files = packagesFolder.GetFilesAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
            }

            return files;
        }

        public IEnumerable<string> ListPackages()
        {
            return ListPackagesStorageFile().Select(p => p.Name);
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

            StorageFile currentPackage = ListPackagesStorageFile().FirstOrDefault(p => p.Name == name);
            if (currentPackage == null)
            {
                ConsoleHelper.WriteError("Package '{name}' not found");
                return false;
            }

            try
            {
                currentPackage.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
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
