using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BlazorMobile.InteropApp.Services
{
    public interface IAssemblyService
    {
        /// <summary>
        /// Return an assembly containing our app packages as a resource
        /// This is required only because we don't have set a direct reference to our project on the shared project
        /// 
        /// An alternative would be to also fetch this assembly directly in the memory Assembly list.
        /// </summary>
        /// <returns></returns>
        Assembly GetAppPackageAssembly();
    }
}
