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
namespace AltoHttp
{
	/// <summary>
	/// Description of RequestHelper.
	/// </summary>
	public static class RequestHelper
	{
		public static HttpWebRequest CreateHttpRequest(RemoteFileInfo info, long start)
		{
			if(start < 0 || start >= info.Length)
				throw new Exception("Range start index is out of the bounds");
			
			if(start > 0 && !info.AcceptRange)
				throw new Exception("Remote file doesn't support ranges");
			
			var request = CreateHttpRequest(info.URL);
			request.AddRange(start);
			
			return request;
		}
		
		
		public static HttpWebRequest CreateHttpRequest(string url)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362";
			request.AllowAutoRedirect = true;
			request.Method = "GET";
			request.Timeout = 10000;
			request.ReadWriteTimeout = 5000;
			return request;
		}
		
		public static HttpWebResponse CreateRequestGetResponse(RemoteFileInfo info, long start, EventHandler<BeforeSendingRequestEventArgs> before, EventHandler<AfterGettingResponseEventArgs> after)
		{
			var request = CreateHttpRequest(info, start);
			if(before != null)
			{
				before(null, new BeforeSendingRequestEventArgs(request));
			}
			var response = (HttpWebResponse)request.GetResponse();
			if(after != null)
			{
				after(null, new AfterGettingResponseEventArgs(response));
			}
			if(response.ContentLength != info.Length - start)
				throw new Exception("Returned content size is wrong; must be " + (info.Length - start));
			
			return response;
		}
		
		
	}
}
