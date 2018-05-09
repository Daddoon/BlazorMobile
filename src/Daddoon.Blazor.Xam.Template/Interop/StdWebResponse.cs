using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Waher.Networking.HTTP;

namespace Daddoon.Blazor.Xam.Template.Interop
{
    public class StdWebResponse : IWebResponse
    {
        private HttpRequest _request = null;
        private HttpResponse _response = null;

        public StdWebResponse(HttpRequest request, HttpResponse response)
        {
            _request = request;
            _response = response;
        }

        public void AddResponseHeader(string key, string value)
        {
            _response.SetHeader(key, value);
        }

        public string GetRequestedPath()
        {
            return _request.SubPath;
        }

        public void SetData(MemoryStream data)
        {
            //Sanity check
            data.Seek(0, SeekOrigin.Begin);

            byte[] content = data.ToArray();
            data.Dispose();

            _response.Write(content);
        }

        public void SetEncoding(string encoding)
        {
            //No-op , encoding is readonly
        }

        public void SetMimeType(string mimetype)
        {
            _response.ContentType = mimetype;
        }

        public void SetReasonPhrase(string reasonPhrase)
        {
            _response.StatusMessage = reasonPhrase;
        }

        public void SetStatutCode(int statutCode)
        {
            _response.StatusCode = statutCode;
        }
    }
}
