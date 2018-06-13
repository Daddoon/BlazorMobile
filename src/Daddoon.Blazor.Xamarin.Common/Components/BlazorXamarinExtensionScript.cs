using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daddoon.Blazor.Xam.Common.Components
{
    public class BlazorXamarinExtensionScript : BlazorComponent
    {
        private static bool _isInitialized = false;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            //This component must be rendered once!
            if (_isInitialized)
                return;

            builder.OpenElement(0, "script");
            builder.AddContent(1, @"
Blazor.registerFunction(""BlazorXamarinRuntimeCheck"", function () {
    if (window.contextBridge == null || window.contextBridge == undefined)
    {
        return false;
    }
    
    return true;
});");
            builder.CloseElement();
            _isInitialized = true;
        }
    }
}
