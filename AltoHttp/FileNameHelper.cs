
using System;
using System.Net;
using System.Text;
using System.IO;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
namespace AltoHttp
{
    /// <summary>
    /// Description of FileNameHelper.
    /// </summary>
    public static class FileNameHelper
    {
        public static string GetFileName(HttpWebResponse resp)
        {
            var cdHeader = resp.Headers["Content-Disposition"];
            var location = resp.Headers["Location"];
            if (!string.IsNullOrEmpty(cdHeader))
            {
                var pattern = string.Format("filename[^;=\n]*=((['\"]).*?{0}|[^;\n]*)", Regex.Escape("2"));
                var omitPattern = "filename[^;\n=]*=(([^'\"])*'')?"; 
                var properFormat = Encoding.UTF8.GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(cdHeader));
                properFormat = HttpUtility.UrlDecode(properFormat);
                properFormat = properFormat.Replace("\"", "");
                var matches = Regex.Matches(properFormat, pattern);
                if (matches.Count > 0)
                {
                    var filename = matches.Cast<Match>().Last().Value;
                    filename = Regex.Replace(filename, omitPattern, "");
                    var temp = HttpUtility.UrlDecode(filename);
                    return temp.ReplaceInvalidChars();
                }
            }
            if (!string.IsNullOrEmpty(location))
            {
                var locUri = new Uri(location);
                var first = Path.GetFileName(locUri.LocalPath);
                if (first.IsCorrectFilename())
                {
                    return HttpUtility.UrlDecode(first);
                }
                else
                {
                    first = Path.GetFileName(locUri.AbsolutePath);
                    return HttpUtility.UrlDecode(first.ReplaceInvalidChars());
                }
            }
            else
            {
                var first = Path.GetFileName(resp.ResponseUri.LocalPath);
                if (first.IsCorrectFilename())
                {
                    return HttpUtility.UrlDecode(first);
                }
                else
                {
                    first = Path.GetFileName(resp.ResponseUri.AbsolutePath);
                    return HttpUtility.UrlDecode(first.ReplaceInvalidChars());
                }
            }
        }

        public static bool HasExtension(this string filename)
        {
            return !string.IsNullOrEmpty(Path.GetExtension(filename));
        }
        public static bool HasInvalidChar(this string filename)
        {
            return Path.GetInvalidFileNameChars().Any(x => filename.Contains(x));
        }
        public static string ReplaceInvalidChars(this string filename)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c.ToString(), "");
            }
            return filename;
        }
        public static bool IsCorrectFilename(this string filename)
        {
            return filename.HasExtension() && !filename.HasInvalidChar();
        }
 
    }

}
