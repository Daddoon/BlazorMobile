using Daddoon.Blazor.Xam.InteropApp.Common.Interfaces;
using Daddoon.Blazor.Xam.InteropApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(XamarinBridge))]
namespace Daddoon.Blazor.Xam.InteropApp.Services
{
    public class XamarinBridge : IXamarinBridge
    {
        public Task DisplayAlert(string title, string msg, string cancel)
        {
            return App.Current.MainPage.DisplayAlert(title, msg, cancel);
        }
    }
}
