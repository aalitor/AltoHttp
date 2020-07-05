/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 14.04.2020
 * Time: 21:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace AltoHttp
{
	/// <summary>
	/// Description of SizeHelper.
	/// </summary>
	public static class SizeHelper
	{
		public static string ToHumanReadableSize(this long byteLen)
		{
			string[] sizes = { "Bytes", "Kb", "Mb", "Gb", "Tb" };
			int order = 0;
			double len = byteLen;
			while (len >= 1024d && order < sizes.Length - 1) {
				order++;
				len = (len / 1024d);
			}

			return String.Format("{0:0.##} {1}", len, sizes[order]);
		}
		public static string ToHumanReadableSize(this int byteLen)
		{
			return ((long)byteLen).ToHumanReadableSize();
		}
		public static double Convert(this long value, SizeUnit from, SizeUnit to)
		{
			int step = to - from;
			
			return value / Math.Pow(1024, step);
		}
	}
}
