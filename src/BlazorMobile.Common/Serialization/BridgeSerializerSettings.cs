using BlazorMobile.Common.Helpers;
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

        public static T Deserialize<T>(string data)
        {
            if (EnvironmentHelper.RunOnCLR())
                data = data.Replace("%CORE%", EnvironmentHelper.GetNetCoreVersion());
            else
                data = data.Replace("%CORE%", "mscorlib");

            return (T)JsonConvert.DeserializeObject<T>(data, BridgeSerializerSettings.GetSerializerSettings());
        }

        public static object Deserialize(string json)
        {
            return JsonConvert.DeserializeObject(json);
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
