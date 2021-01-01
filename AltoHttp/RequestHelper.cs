using AltoHttp.NativeMessages;
/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 14.04.2020
 * Time: 21:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using AltoHttp.Exceptions;
namespace AltoHttp
{
    /// <summary>
    /// Description of RequestHelper.
    /// </summary>
    public static class RequestHelper
    {

        static string[] restrictedHeaders = new[]{
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
        public static HttpWebRequest CreateHttpRequest(RemoteFileInfo info, long start, EventHandler<BeforeSendingRequestEventArgs> before)
        {
            if (start < 0)
                throw new NegativeRangeStartOffsetException("range start offset cannot be negative");

            if (info.Length > 0 && start >= info.Length)
                throw new RangeStartOffsetGreaterThanContentSizeException("Range start offset cannot be greater than or equal to content size");

            if (start > 0 && !info.AcceptRange)
                throw new ResumeDownloadNotSupportedException("Request cannot made if server doesnt supports ranges");

            if(start > 0 && info.Length < 1)
                throw new ResumeDownloadNotSupportedException("Request cannot made if content length is unknown");

            var request = CreateHttpRequest(info.URL, before);
            request.AddRange(start);

            return request;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        public static HttpWebRequest CreateHttpRequest(RemoteFileInfo info, long start, long end, EventHandler<BeforeSendingRequestEventArgs> before)
        {
            if (start < 0)
                throw new NegativeRangeStartOffsetException("range start offset cannot be negative");

            if (info.Length > 0 && start >= info.Length)
                throw new RangeStartOffsetGreaterThanContentSizeException("Range start offset cannot be greater than or equal to content size");

            if (info.Length > 0 && end >= info.Length)
                throw new RangeEndOffsetGreaterThanContentSizeException("Range end offset cannot be greater that or equal to content size");

            if (start > 0 && !info.AcceptRange)
                throw new ResumeDownloadNotSupportedException("Request cannot made if server doesnt supports ranges");

            if (start > 0 && info.Length < 1)
                throw new ResumeDownloadNotSupportedException("Request cannot made if content length is unknown");

            var request = CreateHttpRequest(info.URL, before);
            request.AddRange(start, end);

            return request;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        public static HttpWebRequest CreateHttpRequest(string url, EventHandler<BeforeSendingRequestEventArgs> before)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362";
            request.AllowAutoRedirect = true;
            request.Method = "GET";
            request.Timeout = 10000;
            request.ReadWriteTimeout = 5000;
            request.KeepAlive = false;
            before.Raise(null, new BeforeSendingRequestEventArgs(request));
            return request;
        }
        /// <summary>
        /// Gets the request 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="start"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <param name="chunked"></param>
        /// <returns></returns>
        public static HttpWebResponse CreateRequestGetResponse(RemoteFileInfo info, long start, EventHandler<BeforeSendingRequestEventArgs> before, EventHandler<AfterGettingResponseEventArgs> after, bool chunked)
        {
            HttpWebRequest request = null;

            if (chunked && info.AcceptRange)
            {
                var chunkSize = 1 * 1024 * 1000;
                var endOffset = start + chunkSize - 1;
                var end = Math.Min(endOffset, info.Length - 1);
                request = CreateHttpRequest(info, start, end, before);
                var response = (HttpWebResponse)request.GetResponse();
                after.Raise(null, new AfterGettingResponseEventArgs(response));

                if (response.ContentLength != end - start + 1)
                    throw new WrongContentSizeReturnedException(start, end, response.ContentLength);

                return response;

            }
            else
            {
                request = CreateHttpRequest(info, start, before);
                var response = (HttpWebResponse)request.GetResponse();
                after.Raise(null, new AfterGettingResponseEventArgs(response));

                if (info.Length > 0 && response.ContentLength != info.Length - start)
                    throw new WrongContentSizeReturnedException(start, (info.Length - 1), response.ContentLength);
                return response;
            }
        }
        /// <summary>
        /// Sets the headers in header array
        /// </summary>
        /// <param name="request">Request to set headers</param>
        /// <param name="headers">Header array</param>
        public static void SetHeaders(this HttpWebRequest request, Header[] headers)
        {
            foreach (var item in headers)
                request.SetHeaderValue(item.Name, item.Value);
        }
        /// <summary>
        /// Sets the headers in header collection
        /// </summary>
        /// <param name="request">Request to set headers</param>
        /// <param name="whc">Header collection</param>
        public static void SetHeaders(this HttpWebRequest request, WebHeaderCollection whc)
        {
            foreach (var header in whc.AllKeys)
                request.SetHeaderValue(header, whc[header]);
        }
        /// <summary>
        /// Sets the header values according the .NET restricted headers
        /// </summary>
        /// <param name="request">Request to add headers</param>
        /// <param name="header">Header name</param>
        /// <param name="value">Header value</param>
        public static void SetHeaderValue(this HttpWebRequest request, string header, object value)
        {
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
