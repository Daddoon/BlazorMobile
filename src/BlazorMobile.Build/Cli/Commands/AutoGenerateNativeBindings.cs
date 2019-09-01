using BlazorMobile.Build.Core;
using BlazorMobile.Build.Server.Core;
using Microsoft.Extensions.CommandLineUtils;
using System;

namespace BlazorMobile.Build.Server.Commands
{
    internal class AutoGenerateNativeBindings
    {
        public static void Command(CommandLineApplication command)
        {
            var projectFile = command.Option("--input",
                "The full path to you .csproj file",
                CommandOptionType.SingleValue);

            var intermediateOutputPath = command.Option("--intermediate-output-path",
                "The project intermediate output path",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                if (!projectFile.HasValue() || !intermediateOutputPath.HasValue())
                {
                    command.ShowHelp(command.Name);
                    return 1;
                }

                try
                {
                    NativeBindingsHelper.GenerateNativeBindings(projectFile.Value(), intermediateOutputPath.Value());
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
