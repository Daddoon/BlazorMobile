using Daddoon.Blazor.Xam.Droid.Renderer;
using Daddoon.Blazor.Xam.Services;
using System;
using System.IO;

namespace Daddoon.Blazor.Xam.Droid.Services
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public static class BlazorWebViewService
    {
        private static void InitComponent(Android.App.Activity activity)
        {
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
