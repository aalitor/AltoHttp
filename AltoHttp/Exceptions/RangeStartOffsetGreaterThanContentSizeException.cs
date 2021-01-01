using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltoHttp.Exceptions
{
    public class RangeStartOffsetGreaterThanContentSizeException : Exception
    {
        private string p;

        public RangeStartOffsetGreaterThanContentSizeException(string p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }
    }
}
