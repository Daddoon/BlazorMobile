using BlazorMobile.Build.Android.Core;
using Microsoft.Extensions.CommandLineUtils;
using System;

namespace BlazorMobile.Build.Android.Commands
{
    internal class InjectIFrameListenerWebExtension
    {
        public static void Command(CommandLineApplication command)
        {
            var nugetPackageDir = command.Option("--package-dir",
                "This must be the absolute path to the current NuGet package",
                CommandOptionType.SingleValue);

            var projectFile = command.Option("--input",
                "The full path to you .csproj file",
                CommandOptionType.SingleValue);

            var intermediateOutputPath = command.Option("--intermediate-output-path",
                "The project intermediate output path",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                if (!nugetPackageDir.HasValue() || !intermediateOutputPath.HasValue() || !projectFile.HasValue())
                {
                    command.ShowHelp(command.Name);
                    return 1;
                }

                try
                {
                    IframeListenerWebExtensionInjectorHelper.CopyOrSkipIframeListenerExtension(nugetPackageDir.Value(), intermediateOutputPath.Value(), projectFile.Value());
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
