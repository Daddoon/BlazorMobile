
using BlazorMobile.Build.Server.Commands;
using Microsoft.AspNetCore.Blazor.Build.DevServer.Commands;
using Microsoft.Extensions.CommandLineUtils;

namespace BlazorMobile.Build
{
    static class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "BlazorMobile.Build"
            };
            app.HelpOption("-?|-h|--help");

            app.Command("pack-blazor-app", PackBlazorAppCommand.Command);

            app.Command("client-to-server-blazor-index", ClientToServerBlazorIndexCommand.Command);

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
