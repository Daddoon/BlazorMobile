using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMobile.Common.Components
{
    public class BlazorXamarinExtensionScript : ComponentBase
    {
        private static bool _isInitialized = false;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            //This component must be rendered once!
            if (_isInitialized)
                return;

            builder.OpenElement(0, "script");
            builder.AddContent(1, @"
    window.BlazorXamarinRuntimeCheck = function () {
        if (window.contextBridge == null || window.contextBridge == undefined)
        {
            return false;
        }
    
        return true;
    };
");
            builder.CloseElement();
            _isInitialized = true;
        }
    }
}
