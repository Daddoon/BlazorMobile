using BlazorMobile.Common.Components;
using BlazorMobile.Common.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMobile.InteropBlazorApp
{
    public class MobileApp : App
    {
        public MobileApp() : base()
        {
            //In the first sequence we must only load BlazorMobile component
            //When BlazorMobile will be ready, we will render the app
            BlazorMobileService.OnBlazorMobileLoaded += BlazorMobileService_OnBlazorMobileLoaded;
        }

        private void BlazorMobileService_OnBlazorMobileLoaded(object source, BlazorMobileOnFinishEventArgs args)
        {
            //BlazorMobile is ready. We should call StateHasChanged method in order to call BuildRenderTree again.
            //This time, it should load your app with base.BuildRenderTree() method call.
            BlazorMobileService.HideElementById("placeholder");
            StateHasChanged();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, nameof(BlazorMobileComponent));
            builder.OpenComponent(1, typeof(BlazorMobileComponent));
            builder.CloseComponent();
            builder.CloseElement();

            Console.WriteLine($"{nameof(BlazorMobileService.IsBlazorMobileLoaded)}: {BlazorMobileService.IsBlazorMobileLoaded}");
            if (BlazorMobileService.IsBlazorMobileLoaded)
            {
                base.BuildRenderTree(builder);
            }
        }
    }
}
