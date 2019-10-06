using BlazorMobile.Common;
using BlazorMobile.Components;
using BlazorMobile.Sample.Helpers;
using BlazorMobile.Services;

namespace BlazorMobile.Sample
{
	public partial class App : BlazorApplication
    {
        public App()
        {
            InitializeComponent();

            ServiceRegistrationHelper.RegisterServices();

#if DEBUG
            WebApplicationFactory.EnableDebugFeatures();
#endif
            WebApplicationFactory.SetHttpPort(8888);

            MainPage = new MainPage();
        }
    }
}
