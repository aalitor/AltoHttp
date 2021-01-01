using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp.Exceptions
{
    /// <summary>
    /// Contains exception to be used when range request returned wrong content size
    /// </summary>
    public class WrongContentSizeReturnedException : Exception
    {
        /// <summary>
        /// Start offset of the range
        /// </summary>
        public long Start { get; set; }
        /// <summary>
        /// End offset of the range
        /// </summary>
        public long End { get; set; }
        /// <summary>
        /// Returned content size from the server for the ranged request
        /// </summary>
        public long ReturnedSize { get; set; }
        /// <summary>
        /// The size that was supposed to be returned by the server
        /// </summary>
        public long SizeShouldBe
        {
            get
            {
                return End - Start + 1;
            }
        }
        /// <summary>
        /// Creates an instance of exception
        /// </summary>
        /// <param name="start">Start offset of the range</param>
        /// <param name="end">End offset of the range</param>
        /// <param name="returnedSize">Returned size by the server</param>
        public WrongContentSizeReturnedException(long start, long end, long returnedSize)
        {
            this.Start = start;
            this.End = end;
            this.ReturnedSize = returnedSize;
        }
        /// <summary>
        /// Gets the exception message
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format(
                        @"Range request returned wrong content size
Requested Range = {0}, Returned Size = {1}, Must be = {2}",
                        Start + "-" + End, ReturnedSize, SizeShouldBe);
            }
        }
    }
}
