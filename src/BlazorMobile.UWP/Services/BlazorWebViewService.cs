using BlazorMobile.UWP.Renderer;
using BlazorMobile.Services;
using System;
using System.IO;
using Xamarin.Forms;
using BlazorMobile.Common.Interfaces;

namespace BlazorMobile.UWP.Services
{
    public class BlazorWebViewService
    {
        private static void InitComponent()
        {
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
