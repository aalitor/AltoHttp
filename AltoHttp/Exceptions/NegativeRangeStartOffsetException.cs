using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp.Exceptions
{
    public class NegativeRangeStartOffsetException : Exception
    {
        private string p;

        public NegativeRangeStartOffsetException(string p) : base(p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }
    }
}
