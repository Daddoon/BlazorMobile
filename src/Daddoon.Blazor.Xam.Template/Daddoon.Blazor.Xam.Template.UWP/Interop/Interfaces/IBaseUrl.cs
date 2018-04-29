using Daddoon.Blazor.Xam.Template.Interfaces;
using Daddoon.Blazor.Xam.Template.UWP.Interop.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]
namespace Daddoon.Blazor.Xam.Template.UWP.Interop.Interfaces
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return "ms-appx-web:///";
        }
    }
}
