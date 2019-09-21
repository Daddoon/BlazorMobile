using BlazorMobile.Common.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


[assembly: InternalsVisibleTo("BlazorMobile")]
[assembly: InternalsVisibleTo("BlazorMobile.Web")]
[assembly: InternalsVisibleTo("BlazorMobile.Common")]
namespace BlazorMobile.Common.Interop
{
    [Serializable]
    public class ExceptionDescriptor
    {
        public string Message { get; set; }

        public ExceptionDescriptor()
        {
            Message = string.Empty;
        }

        public ExceptionDescriptor(Exception ex) : this()
        {
            Message = ex.Message;
        }
    }

    [Serializable]
    public class TypeProxy
    {
        public TypeProxy()
        {

        }

        public TypeProxy(Type type)
        {
            SerializedData = BridgeSerializer.Serialize(type);
        }

        public static TypeProxy CreateFromJson(string json)
        {
            return new TypeProxy()
            {
                SerializedData = json
            };
        }

        public string SerializedData { get; set; }

        /// <summary>
        /// Get resolved type from given AssemblyName & TypeName
        /// </summary>
        /// <returns></returns>
        public Type ResolvedType()
        {

            if (string.IsNullOrEmpty(SerializedData))
                throw new NullReferenceException();

            return BridgeSerializer.Deserialize<Type>(this);
        }
    }

    //NOTE: We can actually store Type instead of TypeProxy, as we are serializing
    //But if we need to add more metadata on objects in the futur, it will be more easier to refactore the code this way
    [Serializable]
    public class MethodProxy
    {
        public Guid TaskIdentity { get; set; }

        public bool TaskSuccess { get; set; }

        public bool AsyncTask { get; set; }

        public TypeProxy ReturnType { get; set; }

        public object ReturnValue { get; set; }

        public TypeProxy InterfaceType { get; set; }

        /// <summary>
        /// Only used with UWP that does not support GetInterfaceMap calls with .NET Native toolchain
        /// </summary>
        public string MethodName { get; set; }

        public int MethodIndex { get; set; }

        /// <summary>
        /// Generic Types of the Method, sorted in the correct order
        /// </summary>
        public TypeProxy[] GenericTypes { get; set; }

        public object[] Parameters { get; set; }

        public ExceptionDescriptor ExceptionDescriptor { get; set; }

        public MethodProxy()
        {

        }
    }
}
