using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMobile.Common.Interop
{
    [Serializable]
    public class MessageProxy
    {
        public MessageProxy()
        {

        }

        /// <summary>
        /// Instanciate a standard MessageProxy
        /// </summary>
        /// <param name="messageName"></param>
        /// <param name="args"></param>
        public MessageProxy(string messageName, Type TArgsType, object[] args)
        {
            //Not needed as boolean is false by default
            //But just for reading clarity
            IsJSInvokable = false;

            InteropMethod = messageName;
            InteropParameters = args;
            InteropArgsType = new TypeProxy(TArgsType);
        }

        /// <summary>
        /// Instanciate a JSInvokable MessageProxy
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        public MessageProxy(string assembly, string method, object[] args)
        {
            IsJSInvokable = true;

            InteropAssembly = assembly;
            InteropMethod = method;
            InteropParameters = args;
        }

        public bool IsJSInvokable { get; set; }

        public string InteropAssembly { get; set; }
        public string InteropMethod { get; set; }

        public TypeProxy InteropArgsType { get; set; }

        /// <summary>
        /// Only used as a flag to differenciate it to MethodProxy without deserializing in javascript context
        /// We may use something more robust in the future, like an update MethodProxy class that manage the MessageProxy API
        /// </summary>
        public string MessageProxyToken => nameof(MessageProxyToken);

        public object[] InteropParameters { get; set; }
    }
}
