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
        public async Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            await App.Current.MainPage.DisplayAlert(title, msg, cancel);

            List<string> result = new List<string>()
            {
                "Lorem",
                "Ipsum",
                "Dolorem",
            };

            return result;
        }
    }
}
