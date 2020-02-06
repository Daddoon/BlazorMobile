using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BlazorMobile.Interop;
using Foundation;
using UIKit;

namespace BlazorMobile.iOS.Services
{
    public class ApplicationStoreService : IApplicationStoreService
    {
        public bool AddPackage(string name, Stream content)
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