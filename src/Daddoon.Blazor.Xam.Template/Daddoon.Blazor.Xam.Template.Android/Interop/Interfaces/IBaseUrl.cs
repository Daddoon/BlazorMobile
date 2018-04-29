using Daddoon.Blazor.Xam.Template.Interfaces;
using Daddoon.Blazor.Xam.Template.Droid.Interop.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]
namespace Daddoon.Blazor.Xam.Template.Droid.Interop.Interfaces
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return "file:///android_asset/";
        }
    }
}
