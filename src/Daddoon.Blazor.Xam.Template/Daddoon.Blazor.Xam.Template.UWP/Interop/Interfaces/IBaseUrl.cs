using Daddoon.Blazor.Xam.Template.Interfaces;
using Daddoon.Blazor.Xam.Template.UWP.Interop.Interfaces;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]
namespace Daddoon.Blazor.Xam.Template.UWP.Interop.Interfaces
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            //return "ms-appx-web:///";
        }
    }
}
