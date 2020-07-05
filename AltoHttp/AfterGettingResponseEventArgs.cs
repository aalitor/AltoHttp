/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 4.07.2020
 * Time: 15:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
namespace AltoHttp
{
	/// <summary>
	/// Description of AfterGettingResponseEventArgs.
	/// </summary>
	public class AfterGettingResponseEventArgs : EventArgs
	{
		public AfterGettingResponseEventArgs(WebResponse response)
		{
			Response = response;
		}
		public WebResponse Response{ get; set; }
	}
}
