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

        /// <summary>
        /// This is a last chance of deserialization.
        /// In some scenario, it seem that Deserializer don't even call the CrossPlatformTypeBinder class
        /// and prevent some data to be deserialized. This helper is intended to force assembly rename before deserialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static T FallbackDeserialize<T>(string data)
        {
            if (CrossPlatformTypeBinder.isNetCore)
            {
                data = data?.Replace("mscorlib", "System.Private.CoreLib");
            }
            else
            {
                data = data.Replace("System.Private.CoreLib", "mscorlib");
            }

            return JsonConvert.DeserializeObject<T>(data, BridgeSerializerSettings.GetSerializerSettings());
        }

        public static T Deserialize<T>(ref string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(data, BridgeSerializerSettings.GetSerializerSettings());
            }
            catch (Exception)
            {
                try
                {
                    return FallbackDeserialize<T>(data);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public static T Deserialize<T>(TypeProxy data)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(data.SerializedData, BridgeSerializerSettings.GetSerializerSettings());
            }
            catch (Exception)
            {
                try
                {
                    return FallbackDeserialize<T>(data.SerializedData);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
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
