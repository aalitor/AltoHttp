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
			string[] sizes = { "B", "KB", "MB", "GB", "TB" };
			int order = 0;
			double len = byteLen;
			while (len >= 1024d && order < sizes.Length - 1) {
				order++;
				len = (len / 1024d);
			}

			return String.Format("{0:0.##} {1}", len, sizes[order]);
		}
	}
}
