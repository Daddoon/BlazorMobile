using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Interop
{
    public interface IWebResponse
    {
        string GetRequestedPath();
        void SetMimeType(string mimetype);
        void SetEncoding(string encoding);
        void SetStatutCode(int statutCode);
        void SetReasonPhrase(string reasonPhrase);
        void AddResponseHeader(string key, string value);
        Task SetDataAsync(MemoryStream data);
    }
}
