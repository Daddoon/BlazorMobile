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
                "The path to the wwwroot folder of your project",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                if (!referencesFile.HasValue())
                {
                    command.ShowHelp(command.Name);
                    return 1;
                }

                try
                {
                    BlazorProjectIndexHelper.FindAndReplace(referencesFile.Value());
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
