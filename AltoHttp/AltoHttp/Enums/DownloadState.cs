using System;
using System.Text;

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
