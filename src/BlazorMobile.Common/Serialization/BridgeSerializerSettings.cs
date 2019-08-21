using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Json.Binder;
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
            return JsonConvert.SerializeObject(value, BridgeSerializerSettings.GetSerializerSettings());
        }

        public static T Deserialize<T>(ref string data)
        {
            return JsonConvert.DeserializeObject<T>(data, BridgeSerializerSettings.GetSerializerSettings());
        }

        public static T Deserialize<T>(TypeProxy data)
        {
            return JsonConvert.DeserializeObject<T>(data.SerializedData, BridgeSerializerSettings.GetSerializerSettings());
        }
    }

    internal class BridgeSerializerSettings : JsonSerializerSettings
    {
        private static BridgeSerializerSettings _settings;

        private BridgeSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All; //Required for writing full assembly
            Converters.Insert(0, new PrimitiveJsonConverter()); //Required for keeping full data information when returning unspecified type from JSON like 'object'.
            SerializationBinder = new CrossPlatformTypeBinder(); //Required for resolving correct type assemblies when interoping between .NET Core Runtime & .NET Framework Runtime
        }

        public static BridgeSerializerSettings GetSerializerSettings()
        {
            if (_settings == null)
            {
                _settings = new BridgeSerializerSettings();
            }

            return _settings;
        }
    }
}
