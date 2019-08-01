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

            var distDir = command.Option("--distDir",
                "Path to the Blazor dist folder",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                if (!referencesFile.HasValue()
                || !outputPath.HasValue()
                || !distDir.HasValue())
                {
                    command.ShowHelp(command.Name);
                    return 1;
                }

                try
                {
                    PublishAndZipHelper.PublishAndZip(referencesFile.Value(), outputPath.Value(), distDir.Value());
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
