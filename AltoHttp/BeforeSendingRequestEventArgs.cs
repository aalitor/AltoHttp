/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 4.07.2020
 * Time: 15:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
namespace AltoHttp
{
	/// <summary>
	/// Description of BeforeGettingResponse.
	/// </summary>
	public class BeforeSendingRequestEventArgs : EventArgs
	{
		public BeforeSendingRequestEventArgs(WebRequest req)
		{
			Request = req;
		}
		public WebRequest Request{ get; set; }
	}
}
