/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 14.04.2020
 * Time: 20:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
namespace AltoHttp
{
	/// <summary>
	/// Description of GlobalSettings.
	/// </summary>
	public class GlobalSettings
	{
		public GlobalSettings()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
			| SecurityProtocolType.Tls11
			| SecurityProtocolType.Tls12
			| SecurityProtocolType.Ssl3;
			ServicePointManager.DefaultConnectionLimit = 10;
		}
	}
}
