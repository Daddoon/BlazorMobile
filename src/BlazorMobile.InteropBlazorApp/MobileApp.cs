using BlazorMobile.Common.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMobile.InteropBlazorApp
{
    public class MobileApp : App
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, nameof(BlazorMobileComponent));
            builder.OpenComponent(1, typeof(BlazorMobileComponent));
            builder.CloseComponent();
            builder.CloseElement();

            base.BuildRenderTree(builder);
        }
    }
}
