using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp.Classes
{
    public static class SizeUnitConverter
    {
        public static string ConvertBestScaledSize(this long bytes)
        {
            int unit = 1024;
            bool inBytes = bytes < unit;
            bool inKb = bytes < unit * unit;
            bool inMb = bytes < unit * unit * unit;
            if (inBytes)
            {
                return bytes + " bytes";
            }
            if (inKb)
            {
                return (bytes / 1024d).ToString("0.00") + " kb";
            }
            if (inMb)
            {
                return (bytes / 1024d / 1024).ToString("0.00") + " mb";
            }
            return (bytes / 1024d / 1024 / 1024).ToString("0.00") + " gb";
        }
        public static double ConvertMemorySize(this long size, FromTo fromTo)
        {
            int degree = (int)fromTo;
            if (degree < 0)
            {
                double divide = Math.Pow(1024, -degree);
                return size / divide;
            }
            else
            {
                double multiplier = Math.Pow(1024, degree);
                return size * multiplier;
            }
        }

    }
}
