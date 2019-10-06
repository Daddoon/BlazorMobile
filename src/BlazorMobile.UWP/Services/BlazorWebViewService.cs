using BlazorMobile.UWP.Renderer;
using BlazorMobile.Services;
using System;
using System.IO;
using Xamarin.Forms;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Interop;
using BlazorMobile.UWP.Interop;

namespace BlazorMobile.UWP.Services
{
    public class BlazorWebViewService
    {
        private static void InitComponent()
        {
            DependencyService.Register<IWebViewService, WebViewService>();

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
