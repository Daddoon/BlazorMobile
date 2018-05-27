using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daddoon.Blazor.Xam.Common.Helpers
{
    //Thanks to notour: https://github.com/Arkatufus/akka.net/pull/2

    /// <summary>
    /// Provide helpers to check the environment where the serializer is currently running
    /// </summary>
    public static class EnvironmentHelper
    {
        #region Properties

        /// <summary>
        /// Gets the runtime net core version.
        /// </summary>
        /// <remarks>
        ///     If the RuntimeNetCoreVersion is null the system is running on a .net Classic environment
        /// </remarks>
        private static string RuntimeNetCoreVersion = null;
        private static bool _runTimeNetCoreVersionInitialized = false;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the net core version.
        /// </summary>
        public static string GetNetCoreVersion()
        {
            if (_runTimeNetCoreVersionInitialized == false)
            {
                var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
                //var assemblyPath = assembly.CodeBase.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                if (assembly.GetName().Name.StartsWith("System.Private.CoreLib"))
                {
                    RuntimeNetCoreVersion = "System.Private.CoreLib";
                }

                ////If NetCore.App

                //int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
                //if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
                //{
                //    RuntimeNetCoreVersion = assemblyPath[netCoreAppIndex + 1];
                //    Console.WriteLine("NetCoreVersion: " + RuntimeNetCoreVersion);
                //}
                //else
                //{
                //    //If Net.CoreRuntime
                //    //int netCoreRuntimIndex = assemblyPath.

                //    RuntimeNetCoreVersion = null;
                //    Console.WriteLine("NetCoreVersion: null");
                //}

                
                _runTimeNetCoreVersionInitialized = true;
            }

            return RuntimeNetCoreVersion;
        }

        public static bool RunOnCLR()
        {
            return GetNetCoreVersion() != null;
        }

        #endregion

        private static string _coreAssemblyName = null;
        public static string GetCoreAssemblyName()
        {
            if (_coreAssemblyName == null)
            {
                _coreAssemblyName = typeof(object).GetTypeInfo().Assembly.GetName().Name;
            }

            return _coreAssemblyName;
        }

    }
}
