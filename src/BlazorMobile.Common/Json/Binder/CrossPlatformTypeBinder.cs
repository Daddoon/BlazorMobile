using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;

namespace BlazorMobile.Common.Json.Binder
{
    internal class CrossPlatformTypeBinder : DefaultSerializationBinder
    {
        internal static readonly bool isNetCore = Type.GetType("System.String, System.Private.CoreLib") != null;
        readonly ConcurrentDictionary<string, Type> mappedTypes = new ConcurrentDictionary<string, Type>();

        public override Type BindToType(string assemblyName, string typeName)
        {
            mappedTypes.TryGetValue(typeName, out var type);

            if (type != null)
                return type;

            var originalTypeName = typeName;

            if (isNetCore)
            {
                typeName = typeName?.Replace("mscorlib", "System.Private.CoreLib");
                assemblyName = assemblyName?.Replace("mscorlib", "System.Private.CoreLib");
            }
            else
            {
                typeName = typeName?.Replace("System.Private.CoreLib", "mscorlib");
                assemblyName = assemblyName?.Replace("System.Private.CoreLib", "mscorlib");
            }

            type = base.BindToType(assemblyName, typeName);
            mappedTypes.TryAdd(originalTypeName, type);
            return type;
        }
    }
}
