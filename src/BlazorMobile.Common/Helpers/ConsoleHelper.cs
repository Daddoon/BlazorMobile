using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("BlazorMobile.UWP")]
[assembly: InternalsVisibleTo("BlazorMobile.Android")]
[assembly: InternalsVisibleTo("BlazorMobile.iOS")]
[assembly: InternalsVisibleTo("BlazorMobile")]
namespace BlazorMobile.Common.Helpers
{
    internal static class ConsoleHelper
    {
        public static void WriteLine(string message)
        {
            message = $"INFO: {message}";

            switch (BlazorDevice.RuntimePlatform)
            {
                case BlazorDevice.UWP:
                    Debug.WriteLine(message);
                    break;
                default:
                    Console.WriteLine(message);
                    break;
            }
        }

        public static void WriteError(string message)
        {
            message = $"ERROR: {message}";

            switch (BlazorDevice.RuntimePlatform)
            {
                case BlazorDevice.UWP:
                    Debug.WriteLine(message);
                    break;
                default:
                    Console.WriteLine(message);
                    break;
            }
        }

        public static void WriteException(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            WriteError(ex.Message);
        }
    }
}
