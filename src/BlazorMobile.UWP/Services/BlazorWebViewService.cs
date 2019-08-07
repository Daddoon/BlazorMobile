using BlazorMobile.UWP.Renderer;
using BlazorMobile.Services;
using System;
using System.IO;
using Xamarin.Forms;
using BlazorMobile.Common.Interfaces;
using BlazorMobile.Components;
using System.Threading.Tasks;
using BlazorMobile.Common.Helpers;

namespace BlazorMobile.UWP.Services
{
    public class BlazorWebViewService
    {
        private static void InitComponent()
        {
            ConsoleHelper.UseDebugWriteLine(true);

            BlazorWebView.SetClearWebViewDelegate(async () =>
            {
                try
                {
                    await Windows.UI.Xaml.Controls.WebView.ClearTemporaryWebDataAsync();
                    ConsoleHelper.WriteLine("UWP WebView Temporary Web Data cleared !");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteException(ex);
                }
            });

            BlazorWebViewRenderer.Init();
        }

        /// <summary>
        /// Init the WebApplicationFactory with the given app stream resolver.
        /// Shorthand for <see cref="WebApplicationFactory.RegisterAppStreamResolver" />
        /// </summary>
        /// <param name="appStreamResolver"></param>
        public static void Init(Func<Stream> appStreamResolver)
        {
            InitComponent();
            WebApplicationFactory.Init(appStreamResolver);
        }

        public static void Init()
        {
            InitComponent();
            WebApplicationFactory.Init();
        }
    }
}
