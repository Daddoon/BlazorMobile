using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Daddoon.Blazor.Xam.Template.Interop;

namespace Daddoon.Blazor.Xam.Template.Droid.Interop
{
    public class AndroidWebResponse : IWebResponse
    {
        private WebResourceResponse _response;
        private MemoryStream _data;
        private int _statusCode = 200;
        private string _reasonPhrase = "OK";
        private Dictionary<string, string> _responseHeaders = null;
        private IWebResourceRequest _request = null;
        private string _url = null;

        private void CommonInit()
        {
            _responseHeaders = new Dictionary<string, string>();
            _data = new MemoryStream();
            _response = new WebResourceResponse("text/plain", "UTF8", _data);

            //Adding a default value
            UpdateReasonAndStatusCode();
        }

        public AndroidWebResponse(string url)
        {
            _url = url;
            CommonInit();
        }

        public AndroidWebResponse(IWebResourceRequest request)
        {
            _request = request;
            CommonInit();
        }

        public WebResourceResponse GetWebResourceResponse()
        {
            return _response;
        }

        public void AddResponseHeader(string key, string value)
        {
            if (Android.OS.Build.VERSION.SdkInt > BuildVersionCodes.KitkatWatch)
            {
                if (!_responseHeaders.ContainsKey(key))
                    _responseHeaders.Add(key, string.Empty);

                _responseHeaders[key] = value;

                //Force update reference (just in cache)
                _response.ResponseHeaders = _responseHeaders;
            }
        }

        public string GetRequestedPath()
        {
            if (_url != null)
            {
                var uri = new Uri(_url, UriKind.Absolute);
                return uri.AbsolutePath;
            }
            else if (_request != null)
            {
                return _request.Url.Path;
            }

            //Should not happen
            return string.Empty;
        }

        public void SetData(MemoryStream data)
        {
            //Because Android want an initial Stream in constructor, we only use one reference.
            //Copying data then disposing source memory stream

            //Rewind all
            data.Seek(0, SeekOrigin.Begin);
            _data.Seek(0, SeekOrigin.Begin);

            //Copy
            data.CopyTo(_data);

            //Disposing source
            data.Dispose();

            //Rewind destination
            _data.Seek(0, SeekOrigin.Begin);
        }

        public void SetEncoding(string encoding)
        {
            if (encoding.ToLowerInvariant() == "UTF-8".ToLowerInvariant())
                encoding = "UTF8";

            _response.Encoding = encoding;
        }

        public void SetMimeType(string mimetype)
        {
            _response.MimeType = mimetype;
        }

        private void UpdateReasonAndStatusCode()
        {
            //No-op on Android 4.5 and older

            if (Android.OS.Build.VERSION.SdkInt > BuildVersionCodes.KitkatWatch)
            {

                //We cannot set ReasonPhrase and StatutCode separed on Android.
                //Just using cached value when updating
                _response.SetStatusCodeAndReasonPhrase(_statusCode, _reasonPhrase);
            }
        }

        public void SetReasonPhrase(string reasonPhrase)
        {
            _reasonPhrase = reasonPhrase;
            UpdateReasonAndStatusCode();
        }

        public void SetStatutCode(int statutCode)
        {
            _statusCode = statutCode;
            UpdateReasonAndStatusCode();
        }
    }
}