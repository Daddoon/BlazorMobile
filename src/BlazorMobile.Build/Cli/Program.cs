
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

            app.Command("resolve-dependencies", ResolveRuntimeDependenciesCommand.Command);
            app.Command("write-boot-json", WriteBootJsonCommand.Command);

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
