using BlazorMobile.Build.Server.Core;
using Microsoft.Extensions.CommandLineUtils;
using System;

namespace BlazorMobile.Build.Server.Commands
{
    internal class ClientToServerBlazorIndexCommand
    {
        public static void Command(CommandLineApplication command)
        {
            var referencesFile = command.Option("--input",
                "The path to your project root folder of your project",
                CommandOptionType.SingleValue);

            var projectFile = command.Option("--project-file",
                "The path to your csproj file",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                if (!referencesFile.HasValue() || !projectFile.HasValue())
                {
                    command.ShowHelp(command.Name);
                    return 1;
                }

                try
                {
                    BlazorProjectIndexHelper.FindAndReplace(referencesFile.Value(), projectFile.Value());
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
