using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp.Exceptions
{
    public class RangeEndOffsetGreaterThanContentSizeException :Exception
    {
        private string p;

        public RangeEndOffsetGreaterThanContentSizeException(string p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }

    }
}
