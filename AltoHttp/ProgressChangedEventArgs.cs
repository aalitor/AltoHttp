/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 15.04.2020
 * Time: 21:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace AltoHttp
{
	/// <summary>
	/// Description of ProgressChangedEventArgs.
	/// </summary>
	public class ProgressChangedEventArgs : EventArgs
	{
		public ProgressChangedEventArgs(int speed, double progress, long totalBytesReceived)
		{
			SpeedInBytes = speed;
			Progress = progress;
			TotalBytesReceived = totalBytesReceived;
		}
		
		public int SpeedInBytes{get;set;}
		public double Progress{get;set;}
		public long TotalBytesReceived{ get; set; }
	}
}
