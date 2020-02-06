using BlazorMobile.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BlazorMobile.UWP.Services
{
    public class ApplicationStoreService : IApplicationStoreService
    {
        private bool FileExist(string fileName)
        {
            var item = ApplicationData.Current.LocalCacheFolder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            return item != null;
        }

        public bool AddPackage(string name, Stream content)
        {
            if (content == null)
            {
                throw new NullReferenceException($"{nameof(content)} cannot be null");
            }

            string relativePath = "packagestore" + Path.DirectorySeparatorChar + name;

            if (FileExist(relativePath))
            {
                throw new InvalidOperationException($"{nameof(name)} already exist on device");
            }

            StorageFolder storageFolder = ApplicationData.Current.LocalCacheFolder;
            StorageFile finalFile = storageFolder.CreateFileAsync(relativePath, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            MemoryStream ms = new MemoryStream();
            content.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);

            FileIO.WriteBytesAsync(finalFile, ms.ToArray()).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            ms.Dispose();

            return true;
        }

        public Func<Stream> GetPackageStreamResolver(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ListPackages()
        {
            throw new NotImplementedException();
        }

        public bool RemovePackage(string name)
        {
            throw new NotImplementedException();
        }
    }
}
