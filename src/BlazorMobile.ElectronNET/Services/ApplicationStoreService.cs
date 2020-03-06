using BlazorMobile.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMobile.ElectronNET.Services
{
    public class ApplicationStoreService : IApplicationStoreService
    {
        public bool AddPackage(string name, Stream content)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddPackageAsync(string name, Stream content)
        {
            throw new NotImplementedException();
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
