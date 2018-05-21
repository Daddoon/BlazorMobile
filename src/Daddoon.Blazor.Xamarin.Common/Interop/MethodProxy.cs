using Daddoon.Blazor.Xam.Common.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


[assembly: InternalsVisibleTo("Daddoon.Blazor.Xamarin")]
namespace Daddoon.Blazor.Xam.Common.Interop
{
    internal class TypeProxy
    {
        public TypeProxy()
        {

        }

        public TypeProxy(Type type)
        {
            SerializedData = BridgeSerializer.Serialize(type);
        }

        private string SerializedData { get; set; }

        /// <summary>
        /// Get resolved type from given AssemblyName & TypeName
        /// </summary>
        /// <returns></returns>
        public Type ResolvedType()
        {
            try
            {
                if (string.IsNullOrEmpty(SerializedData))
                    return null;

                var type = BridgeSerializer.Deserialize<Type>(SerializedData);
                return type;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    //NOTE: We can actually store Type instead of TypeProxy, as we are serializing
    //But if we need to add more metadata on objects in the futur, it will be more easier to refactore the code this way
    internal class MethodProxy
    {
        public TypeProxy InterfaceType { get; set; } 
        public int MethodIndex { get; set; }

        /// <summary>
        /// Generic Types of the Method, sorted in the correct order
        /// </summary>
        public TypeProxy[] GenericTypes { get; set; }

        public object[] Parameters { get; set; }

        public MethodProxy()
        {

        }
    }
}
