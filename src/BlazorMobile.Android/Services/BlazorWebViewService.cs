using BlazorMobile.Components;
using BlazorMobile.Droid.Interop;
using BlazorMobile.Droid.Renderer;
using BlazorMobile.Interop;
using BlazorMobile.Services;
using System;
using System.IO;
using Xamarin.Forms;

namespace BlazorMobile.Droid.Services
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public static class BlazorWebViewService
    {
        private static void InitComponent(Android.App.Activity activity)
        {
            DependencyService.Register<IWebViewService, WebViewService>();

            //Instanciate GeckoView type in BlazorMobile assemnly for Android
            BlazorWebViewFactory.SetInternalBlazorGeckoViewType(typeof(BlazorGeckoView));

            BlazorGeckoViewRenderer.Init(activity);
        }

        /// <summary>
        /// Init the WebApplicationFactory with the given app stream resolver.
        /// Shorthand for <see cref="WebApplicationFactory.RegisterAppStreamResolver" />
        /// </summary>
        /// <param name="appStreamResolver"></param>
        public static void Init(Android.App.Activity activity, Func<Stream> appStreamResolver)
        {
            InitComponent(activity);
            WebApplicationFactory.Init(appStreamResolver);
        }

        public static void Init(Android.App.Activity activity)
        {
            InitComponent(activity);
            WebApplicationFactory.Init();
        }
    }
}
