// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BlazorMobile.Build.Core;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;

namespace Microsoft.AspNetCore.Blazor.Build.DevServer.Commands
{
    internal class PackBlazorAppCommand
    {
        public static void Command(CommandLineApplication command)
        {
            var referencesFile = command.Option("--input",
                "The path to a Blazor project .csproj",
                CommandOptionType.SingleValue);

            var outputPath = command.Option("--outputPath",
                "Path to the output path",
                CommandOptionType.SingleValue);

            var configuration = command.Option("--configuration",
                "The requested build configuration (Debug or Release)",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                if (!referencesFile.HasValue()
                || !outputPath.HasValue()
                || !configuration.HasValue()
                || (!string.Equals(configuration.Value(), "Debug"
, StringComparison.InvariantCultureIgnoreCase)
                && !string.Equals(configuration.Value(), "Release", StringComparison.InvariantCultureIgnoreCase)))
                {
                    command.ShowHelp(command.Name);
                    return 1;
                }

                try
                {
                    PublishAndZipHelper.PublishAndZip(referencesFile.Value(), outputPath.Value(), configuration.Value());
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
