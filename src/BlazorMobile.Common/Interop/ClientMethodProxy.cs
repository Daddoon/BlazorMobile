using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMobile.Common.Interop
{
    [Serializable]
    public class ClientMethodProxy
    {
        public string InteropAssembly { get; set; }
        public string InteropMethod { get; set; }
        public string ClientMethodProxyIdentifier
        {
            get
            {
                return "ClientMethodProxy";
            }
        }
        public object[] InteropParameters { get; set; }
    }
}
