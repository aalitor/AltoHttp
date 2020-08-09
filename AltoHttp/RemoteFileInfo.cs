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
using System.Collections.Generic;
using System.IO;
namespace AltoHttp
{
	/// <summary>
	/// Description of RemoteFileInfo.
	/// </summary>
	public class RemoteFileInfo
	{
		public bool AcceptRange{ get; set; }
		public string ServerFileName{ get; set; }
		public string URL{ get; set; }
		public long Length{ get; set; }
		
		public static RemoteFileInfo Get(string url, EventHandler<BeforeSendingRequestEventArgs> before, EventHandler<AfterGettingResponseEventArgs> after)
		{
			
			using (var response = (HttpWebResponse)RequestHelper.CreateHttpRequest(url, before).GetResponse())
			{
				after.Raise(null, new AfterGettingResponseEventArgs(response));
				var headers = response.Headers;
				return new RemoteFileInfo()
				{
					AcceptRange = headers.AllKeys.Any(x => x.ToLower().Contains("range") && headers[x].Contains("bytes")),
					ServerFileName = FileNameHelper.GetFileName(response),
					URL = url,
					Length = response.ContentLength
				};
			}
		}
		
		public override string ToString()
		{
			return string.Format("[AcceptRange={0}, ServerFileName={1}, URL={2}]", AcceptRange, ServerFileName, URL);
		}

	}
}
