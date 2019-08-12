using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BlazorMobile.Common.Serialization
{
    /// <summary>
    /// Just a simple Wrapper around Newtonsoft.Json for an easier refactoring if needed
    /// </summary>
    public static class BridgeSerializer
    {
        public static string Serialize(object value)
        {
            string data = JsonConvert.SerializeObject(value, BridgeSerializerSettings.GetSerializerSettings());

            if (EnvironmentHelper.RunOnCLR())
                data = data.Replace("System.Private.CoreLib", "%CORE%");
            else
                data = data.Replace("mscorlib", "%CORE%");

            return data;
        }

        private static void JsonClrFixer()
        {

        }

        public static T Deserialize<T>(ref string data)
        {
            if (EnvironmentHelper.RunOnCLR())
                data = data.Replace("%CORE%", EnvironmentHelper.GetNetCoreVersion());
            else
                data = data.Replace("%CORE%", "mscorlib");

            return JsonConvert.DeserializeObject<T>(data, BridgeSerializerSettings.GetSerializerSettings());
        }

        public static T Deserialize<T>(TypeProxy data)
        {
            string fixedJson;

            if (EnvironmentHelper.RunOnCLR())
                fixedJson = data.SerializedData.Replace("%CORE%", EnvironmentHelper.GetNetCoreVersion());
            else
                fixedJson = data.SerializedData.Replace("%CORE%", "mscorlib");

            return JsonConvert.DeserializeObject<T>(fixedJson, BridgeSerializerSettings.GetSerializerSettings());
        }
    }

    public class BridgeSerializerSettings : JsonSerializerSettings
    {
        public static BridgeSerializerSettings GetSerializerSettings()
        {
            var _settings = new BridgeSerializerSettings();
            //_settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            _settings.TypeNameHandling = TypeNameHandling.All;
            _settings.Converters.Insert(0, new PrimitiveJsonConverter());

            return _settings;
        }
    }
}
