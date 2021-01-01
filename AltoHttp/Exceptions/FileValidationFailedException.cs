using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp.Exceptions
{
    public class FileValidationFailedException : Exception
    {
        public string FilePath { get; set; }
        public string LastChecksum { get; set; }
        public string CurrentChecksum { get; set; }
        public FileValidationFailedException(string filePath, string lastChecksum, string currentChecksum)
        {
            this.FilePath = filePath;
            this.LastChecksum = lastChecksum;
            this.CurrentChecksum = currentChecksum;
        }

        public override string Message
        {
            get
            {
                return "File validation failed to resume download";
            }
        }
    }
}
