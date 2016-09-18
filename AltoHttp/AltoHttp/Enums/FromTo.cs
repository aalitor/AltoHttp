using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp.Classes
{
    public enum FromTo
    {
        BytesToKb = -1,
        BytesToMb = -2,
        BytesToGb = -3,
        KbToBytes = 1,
        KbToMb = -1,
        KbToGb = -2,
        MbToBytes = 2,
        MbToKb = 1,
        MbToGb = -1,
        GbToBytes = 3,
        GbToKb = 2,
        GbToMb = 1
    }
}
