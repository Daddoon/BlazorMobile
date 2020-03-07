using BlazorMobile.Common.Helpers;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("BlazorMobile.Android")]
[assembly: InternalsVisibleTo("BlazorMobile.iOS")]
[assembly: InternalsVisibleTo("BlazorMobile.UWP")]
[assembly: InternalsVisibleTo("BlazorMobile.ElectronNET")]
namespace BlazorMobile.Helper
{
    /// <summary>
    /// This class is used for the ApplicationStore service for platforms where the main methods
    /// are usable without code change. At the moment, only UWP is requesting a full different code base
    /// due to API limitation and changes
    /// </summary>
    internal static class ApplicationStoreHelper
    {
        private static string _cachedPackageFolder = null;

        internal static void SetPackageFolder(string packageFolder)
        {
            _cachedPackageFolder = packageFolder;
        }

        internal static string GetPackagesFolder()
        {
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

        internal static bool FileExist(string fileName)
        {
            string packageFolder = GetPackagesFolder();
            return File.Exists(Path.Combine(packageFolder, fileName));
        }

        internal static bool AddPackage(string name, Stream content)
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

        internal static Task<bool> AddPackageAsync(string name, Stream content)
        {
            return Task.FromResult(AddPackage(name, content));
        }

        internal static Func<Stream> GetPackageStreamResolver(string name)
        {
            string packageFile = ListPackagesStorageFile().FirstOrDefault(p => Path.GetFileName(p) == name);
            if (packageFile == null)
            {
                return null;
            }

            Stream package = File.OpenRead(packageFile);

            return () => package;
        }

        internal static IEnumerable<string> ListPackagesStorageFile()
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

        internal static IEnumerable<string> ListPackages()
        {
            return ListPackagesStorageFile().Select(Path.GetFileName);
        }

        internal static bool RemovePackage(string name)
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
