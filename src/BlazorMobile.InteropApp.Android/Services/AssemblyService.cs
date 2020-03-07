using BlazorMobile.InteropApp.AppPackage.Types;
using BlazorMobile.InteropApp.Services;
using System.Reflection;

namespace BlazorMobile.InteropApp.Droid.Services
{
    public class AssemblyService : IAssemblyService
    {
        public Assembly GetAppPackageAssembly()
        {
            return typeof(AppPackageProjectType).Assembly;
        }
    }
}
