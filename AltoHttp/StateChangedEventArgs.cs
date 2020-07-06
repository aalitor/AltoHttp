/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 15.04.2020
 * Time: 22:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace AltoHttp
{
	/// <summary>
	/// Description of StateChangedEventArgs.
	/// </summary>
	public class StatusChangedEventArgs : EventArgs
	{
		public StatusChangedEventArgs(Status status)
		{
			this.Status = status;
		}
		public Status Status{ get; set; }
	}
}
