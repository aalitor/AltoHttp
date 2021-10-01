using System;
using System.Linq;
using System.Net;

namespace AltoHttp
{

    /// <summary>
    /// Provides static methods for headers
    /// </summary>
    public static class HttpHeaderHelper
    {
        /// <summary>
        /// Sets header values to request as given in the header collection
        /// </summary>
        /// <param name="request">Request to set the headers</param>
        /// <param name="whc">Headers collection to set into request</param>
        public static void SetHeadersProperly(this HttpWebRequest request, WebHeaderCollection whc)
        {
            foreach (var header in whc.AllKeys)
                request.SetHeaderValueProperly(header, whc[header]);
        }
        /// <summary>
        /// Sets header value using the proper method
        /// </summary>
        /// <param name="request">Request to set the header</param>
        /// <param name="header">Header to set</param>
        /// <param name="value">Value of the header</param>
        public static void SetHeaderValueProperly(this HttpWebRequest request, string header, object value)
        {
            var restrictedHeaders = new[]{
                "Accept",
                "Connection",
                "Content-Length",
                "Content-Type",
                "Date",
                "Expect",
                "Host",
                "If-Modified-Since",
                "Range",
                "Referer",
                "Transfer-Encoding",
                "User-Agent",
                "Proxy-Connection"
            };

            if (restrictedHeaders.Contains(header))
            {
                switch (header)
                {
                    case "Accept":
                        request.Accept = (string)value;
                        break;
                    case "Connection":
                        request.Connection = (string)value;
                        break;
                    case "Content-Length":
                        request.ContentLength = (long)value;
                        break;
                    case "Content-Type":
                        request.ContentType = (string)value;
                        break;
                    case "Date":
                        request.Date = (DateTime)value;
                        break;
                    case "Expect":
                        request.Expect = (string)value;
                        break;
                    case "Host":
                        request.Host = (string)value;
                        break;
                    case "If-Modified-Since":
                        request.IfModifiedSince = (DateTime)value;
                        break;
                    //case "Range":
                    //    request.AddRange
                    //    break;
                    case "Referer":
                        request.Referer = (string)value;
                        break;
                    case "Transfer-Encoding":
                        request.TransferEncoding = (string)value;
                        break;
                    case "User-Agent":
                        request.UserAgent = (string)value;
                        break;
                    case "Proxy-Connection":
                        request.Proxy = (IWebProxy)value;
                        break;
                }
            }
            else
                request.Headers[header] = (string)value;

        }
    }
}
