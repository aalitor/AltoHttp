/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 14.04.2020
 * Time: 22:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace AltoHttp
{
	/// <summary>
	/// Description of Status.
	/// </summary>
	public enum Status
	{
		Downloading,
		Paused,
		GettingResponse,
		SendingRequest,
		Completed,
		ErrorOccured,
		GettingHeaders
	}
}
