using Daddoon.Blazor.Xam.UWP.Renderer;
using Daddoon.Blazor.Xam.Services;
using System;
using System.IO;

namespace Daddoon.Blazor.Xam.UWP.Services
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
