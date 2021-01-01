using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp.Exceptions
{
    public class ResumeDownloadNotSupportedException : Exception
    {
        public string Url { get; set; }
        public ResumeDownloadNotSupportedException(string url)
        {
            this.Url = url;
        }

        public ResumeDownloadNotSupportedException()
        {
            // TODO: Complete member initialization
        }

        public override string Message
        {
            get
            {
                return "Resume not supported for the remote source : " + Url;
            }
        }
    }
}
