using BlazorMobile.Common;
using BlazorMobile.Sample.Common.Interfaces;
using BlazorMobile.Sample.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(XamarinBridge))]
namespace BlazorMobile.Sample.Services
{
    public class XamarinBridge : IXamarinBridge
    {
        public Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            if (BlazorDevice.IsElectronNET())
            {
                Console.WriteLine(msg);
            }
            else
            {
                App.Current.MainPage.DisplayAlert(title, msg, cancel);
            }

            List<string> result = new List<string>()
            {
                "Lorem",
                "Ipsum",
                "Dolorem",
            };

            return Task.FromResult(result);
        }
    }
}
