/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 14.04.2020
 * Time: 21:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
namespace AltoHttp
{
	/// <summary>
	/// Description of FileHelper.
	/// </summary>
	public static class FileHelper
	{
		public static FileStream CheckFile(string filePath, bool append)
		{
			var exists = File.Exists(filePath);
			if(append)
			{
				if(!exists) throw new Exception("File not exists to append data");
				else return new FileStream(filePath, FileMode.Append, FileAccess.Write);
			}
			else
			{
				if(!exists) return new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
				else return new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite);
			}
		}
	}
}
