using Android.Webkit;
using Daddoon.Blazor.Xam.Components;
using Daddoon.Blazor.Xam.Droid.Renderer;
using Daddoon.Blazor.Xam.Interop;
using Java.Interop;
using Java.Lang;
using System;
using Xamarin.Forms.Platform.Android;

public class JSBridge : Java.Lang.Object
{
    readonly WeakReference<BlazorWebViewRenderer> blazorViewRenderer;

    public JSBridge(BlazorWebViewRenderer blazorRenderer)
    {
        blazorViewRenderer = new WeakReference<BlazorWebViewRenderer>(blazorRenderer);
    }

    [JavascriptInterface]
    [Export("invokeAction")]
    public void InvokeAction(string data)
    {
        BlazorWebViewRenderer blazorRenderer;

        if (blazorViewRenderer != null && blazorViewRenderer.TryGetTarget(out blazorRenderer))
        {
            ContextBridge.BridgeEvaluator((BlazorWebView)blazorRenderer.Element, data);
        }
    }
}