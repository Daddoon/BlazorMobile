using System;
using System.Collections.Generic;
using System.Text;

namespace Daddoon.Blazor.Xam.Interop
{
    public class CsharpProxy
    {
        /// <summary>
        /// Assembly name of the used interface. Should be a common shared interface between Blazor and Xamarin project
        /// </summary>
        public string AssemblyName { get; set; }
        public string MethodName { get; set; }
        public bool IsGeneric { get; set; }

        public CsharpProxy()
        {

        }
    }

    public static class ContextBridge
    {
        public static void Send()
        {

        }

        public static void Receive(string csharpProxy)
        {

        }
    }
}
