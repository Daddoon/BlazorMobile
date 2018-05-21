using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daddoon.Blazor.Xam.Common.Serialization
{
    /// <summary>
    /// Just a simple Wrapper around Newtonsoft.Json for an easier refactoring if needed
    /// </summary>
    public static class BridgeSerializer
    {
        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, BridgeSerializerSettings.GetSerializerSettings());
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, BridgeSerializerSettings.GetSerializerSettings());
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
            _settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            _settings.TypeNameHandling = TypeNameHandling.All;
            _settings.Converters.Insert(0, new PrimitiveJsonConverter());

            return _settings;
        }
    }
}
