using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BlazorMobile.Interop
{
    public interface IApplicationStoreService
    {
        /// <summary>
        /// Add the given Stream as a package in a data store on the device, with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        bool AddPackage(string name, Stream content);

        /// <summary>
        /// Remove the package with the given name from the data store of the device
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool RemovePackage(string name);

        /// <summary>
        /// List available packages in the data store of the device
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> ListPackages();

        Func<Stream> GetPackageStreamResolver(string name);
    }
}
