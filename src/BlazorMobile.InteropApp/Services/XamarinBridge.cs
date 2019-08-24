using BlazorMobile.Common;
using BlazorMobile.InteropApp.Common.Interfaces;
using BlazorMobile.InteropApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(XamarinBridge))]
namespace BlazorMobile.InteropApp.Services
{
    public class XamarinBridge : IXamarinBridge
    {
        public Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            if (!BlazorDevice.IsElectronNET())
            {
                App.Current.MainPage.DisplayAlert(title, msg, cancel);
            }
            else
            {
                Console.WriteLine(msg);
            }

            List<string> result = new List<string>()
            {
                "Lorem",
                "Ipsum",
                "Dolorem",
            };

            return Task.FromResult(result);
        }

        public Task CallFaultyTask()
        {
            throw new InvalidOperationException("This is an expected exception");
        }
    }
}
