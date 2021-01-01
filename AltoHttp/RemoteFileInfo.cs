/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 14.04.2020
 * Time: 20:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Linq;

namespace AltoHttp
{
	/// <summary>
	/// Description of RemoteFileInfo.
	/// </summary>
	public class RemoteFileInfo
	{
        /// <summary>
        /// Gets if server supports resume
        /// </summary>
		public bool AcceptRange{ get; set; }
        /// <summary>
        /// Gets the filename that server returns
        /// </summary>
		public string ServerFileName{ get; set; }
        /// <summary>
        /// Gets the remote file url
        /// </summary>
		public string URL{ get; set; }
        /// <summary>
        /// Gets the content size of the remote file
        /// </summary>
		public long Length{ get; set; }
        /// <summary>
        /// Gets if the server sends file in chunked streams which is max 1MB
        /// </summary>
        public bool IsChunked { get; set; }
        /// <summary>
        /// Gets the remote file informations from the server
        /// </summary>
        /// <param name="url">Remote file url</param>
        /// <param name="before">Event to be raised before sending request</param>
        /// <param name="after">Event to be raised after getting response</param>
        /// <returns></returns>
		public static RemoteFileInfo Get(string url, EventHandler<BeforeSendingRequestEventArgs> before, EventHandler<AfterGettingResponseEventArgs> after)
		{
            var request = (HttpWebRequest)RequestHelper.CreateHttpRequest(url, before);
            request.AddRange(0);
			using (var response = (HttpWebResponse)request.GetResponse())
			{
				after.Raise(null, new AfterGettingResponseEventArgs(response));
				var headers = response.Headers;


                var serverFileName = FileNameHelper.GetFileName(response);
                
                var contentSize = response.ContentLength;
                var contentRange = response.Headers[HttpResponseHeader.ContentRange];
                if (contentSize < 1 && !string.IsNullOrEmpty(contentRange))
                {
                    var parts = contentRange.Split('/');

                    if (parts.Length > 1)
                        long.TryParse(parts[1], out contentSize);
                }

                var transferEncodingHeader = response.Headers[HttpResponseHeader.TransferEncoding];
                var isChunked = false;
                if(transferEncodingHeader != null)
                    isChunked = transferEncodingHeader.ToLower() == "chunked";
                var acceptRanges = headers.AllKeys.Any(x => x.ToLower().Contains("range") && headers[x].Contains("bytes"))
                    && contentSize > 0;
				return new RemoteFileInfo
                {
                    AcceptRange = acceptRanges,
                    ServerFileName = serverFileName,
					URL = url,
					Length = contentSize,
                    IsChunked = isChunked
				};
			}
		}
		/// <summary>
		/// String format of the info
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[AcceptRange={0}, ServerFileName={1}, URL={2}, IsChunked={3}]", AcceptRange, ServerFileName, URL, IsChunked);
		}
        /// <summary>
        /// Gets the content size in user readable format, -1 equals Unknown
        /// </summary>
        public string ContentLengthText
        {
            get
            {
                return Length > 0 ? Length.ToHumanReadableSize() : "Unknown";
            }
        }
	}
}
