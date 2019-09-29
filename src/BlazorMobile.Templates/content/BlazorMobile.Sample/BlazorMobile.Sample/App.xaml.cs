using BlazorMobile.Common;
using BlazorMobile.Components;
using BlazorMobile.Services;

namespace BlazorMobile.Sample
{
	public partial class App : BlazorApplication
    {
        public App()
        {
            InitializeComponent();

#if DEBUG
            WebApplicationFactory.EnableDebugFeatures();
#endif
            WebApplicationFactory.SetHttpPort(8888);

            MainPage = new MainPage();
        }
    }
}
