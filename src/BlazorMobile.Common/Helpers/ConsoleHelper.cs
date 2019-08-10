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
        private static bool _useDebugWriteLine = false;

        /// <summary>
        /// If true, use Debug.WriteLine instead of Console.WriteLine
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public static void UseDebugWriteLine(bool enabled)
        {
            _useDebugWriteLine = enabled;
        }

        public static void WriteLine(string message)
        {
            message = $"INFO: {message}";

            if (_useDebugWriteLine)
            {
                Debug.WriteLine(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public static void WriteError(string message)
        {
            message = $"ERROR: {message}";

            if (_useDebugWriteLine)
            {
                Debug.WriteLine(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public static void WriteException(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            WriteLine($"ERROR: {ex.Message}");
        }
    }
}
