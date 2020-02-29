using BlazorMobile.Interop;
using BlazorMobile.iOS.Interop;
using BlazorMobile.iOS.Renderer;
using BlazorMobile.Services;
using System;
using System.IO;
using UIKit;
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

        /// <summary>
        /// This option is required if your are deploying your app on iOS 13.
        /// Current iOS 13 release have some issue with memory on Safari that prevent Blazor to be loaded.
        /// Enabling this option will force your app to load an alternate blazor.webassembly.js file that
        /// give additionnal time at boot time during some heavy load, to prevent an
        /// 'Unhandled Promise Rejection: RangeError: Maximum call stack size exceeded' on Safari.
        /// </summary>
        public static void EnableDelayedStartPatch()
        {
            WebApplicationFactory.PlatformSpecific.EnableDelayedStartPatch(true);
        }
    }
}
