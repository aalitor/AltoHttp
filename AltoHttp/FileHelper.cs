using AltoHttp.Exceptions;
/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 14.04.2020
 * Time: 21:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
namespace AltoHttp
{
	/// <summary>
	/// Description of FileHelper.
	/// </summary>
	public static class FileHelper
	{
        /// <summary>
        /// Checks and creates filestream according to the file if exists or not to append or not
        /// </summary>
        /// <param name="filePath">Filepath to check</param>
        /// <param name="append">Check file to append bytes or not</param>
        /// <returns></returns>
		public static FileStream CheckFile(string filePath, bool append, string lastChecksum)
		{
			var exists = File.Exists(filePath);
			if(append)
			{
				if(!exists) throw new Exception("File not exists to resume");

                var currentChecksum = CalculateMD5(filePath);
                if (currentChecksum != lastChecksum)
                    throw new FileValidationFailedException(filePath, lastChecksum, currentChecksum);
                else return new FileStream(filePath, FileMode.Append, FileAccess.Write);
			}
			else
			{
				if(!exists) return new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
				else return new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite);
			}
		}

        public static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        
	}
}
