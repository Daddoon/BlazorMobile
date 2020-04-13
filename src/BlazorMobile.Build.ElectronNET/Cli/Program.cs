using BlazorMobile.Build.ElectronNET.Commands;
using Microsoft.Extensions.CommandLineUtils;

namespace BlazorMobile.Build
{
    static class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "BlazorMobile.Build.ElectronNET"
            };
            app.HelpOption("-?|-h|--help");

            app.Command("sync-blazormobile-assemblies", SyncMissingBlazorMobileAssemblies.Command);

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
