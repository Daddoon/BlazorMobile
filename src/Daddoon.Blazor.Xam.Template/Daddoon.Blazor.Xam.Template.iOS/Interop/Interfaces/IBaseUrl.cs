using Daddoon.Blazor.Xam.Template.Interfaces;
using Daddoon.Blazor.Xam.Template.iOS.Interop.Interfaces;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]
namespace Daddoon.Blazor.Xam.Template.iOS.Interop.Interfaces
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}
