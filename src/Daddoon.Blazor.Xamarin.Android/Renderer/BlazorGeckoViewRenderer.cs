
using Daddoon.Blazor.Xam.Components;
using Daddoon.Blazor.Xam.Droid.Renderer;
using Xam.Droid.GeckoView.Forms.Droid.Renderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BlazorGeckoView), typeof(BlazorGeckoViewRenderer))]
namespace Daddoon.Blazor.Xam.Droid.Renderer
{
    public class BlazorGeckoViewRenderer : GeckoViewRenderer
    {
    }
}