using BlazorMobile.Build.Android.Commands;
using Microsoft.Extensions.CommandLineUtils;

namespace BlazorMobile.Build
{
    static class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "BlazorMobile.Build.Android"
            };
            app.HelpOption("-?|-h|--help");

            app.Command("inject-iframe-listener-webextension", InjectIFrameListenerWebExtension.Command);

            if (args.Length > 0)
            {
                return app.Execute(args);
            }
            else
            {
                app.ShowHelp();
                return 0;
            }
        }
    }
}
