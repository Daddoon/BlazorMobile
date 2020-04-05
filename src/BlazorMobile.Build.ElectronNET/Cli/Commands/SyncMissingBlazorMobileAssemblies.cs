using BlazorMobile.Build.ElectronNET.Core;
using Microsoft.Extensions.CommandLineUtils;
using System;

namespace BlazorMobile.Build.ElectronNET.Commands
{
    internal class SyncMissingBlazorMobileAssemblies
    {
        public static void Command(CommandLineApplication command)
        {
            var binDir = command.Option("--input",
                "The path to the project bin directory",
                CommandOptionType.SingleValue);

            var intermediateOutputPath = command.Option("--intermediate-output-path",
                "The project intermediate output path",
                CommandOptionType.SingleValue);

            var projectPath = command.Option("--project-path",
                "The path to the project .csproj file",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                if (!intermediateOutputPath.HasValue() || !binDir.HasValue() || !projectPath.HasValue())
                {
                    command.ShowHelp(command.Name);
                    return 1;
                }

                try
                {
                    SyncMissingBlazorMobileAssembliesHelper.CopyMissingAssemblies(projectPath.Value(), intermediateOutputPath.Value(), binDir.Value());
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    return 1;
                }
            });
        }
    }
}
