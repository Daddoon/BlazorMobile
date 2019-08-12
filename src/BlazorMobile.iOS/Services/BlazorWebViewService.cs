using BlazorMobile.Interop;
using BlazorMobile.iOS.Interop;
using BlazorMobile.iOS.Renderer;
using BlazorMobile.Services;
using System;
using System.IO;
using Xamarin.Forms;

namespace BlazorMobile.iOS.Services
{
    public class BlazorWebViewService
    {
        private static void InitComponent()
        {
            DependencyService.Register<IWebViewService, WebViewService>();
            BlazorWebViewRenderer.BlazorInit();
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
