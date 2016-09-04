using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp
{
    public enum DownloadState
    {
        Started, 
        Paused,
        Downloading, 
        Completed, 
        Cancelled,
        ErrorOccured
    }
}
