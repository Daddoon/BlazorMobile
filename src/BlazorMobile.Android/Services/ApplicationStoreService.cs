using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlazorMobile.Interop;

namespace BlazorMobile.Droid.Services
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