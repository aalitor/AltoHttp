using System;
using System.Text;

namespace AltoHttp
{
    /// <summary>
    /// Download states
    /// </summary>
    public enum DownloadState
    {
        /// <summary>
        /// Download is started
        /// </summary>
        Started, 
        /// <summary>
        /// Download is paused
        /// </summary>
        Paused,
        /// <summary>
        /// Download is going on
        /// </summary>
        Downloading, 
        /// <summary>
        /// Download is completed
        /// </summary>
        Completed, 
        /// <summary>
        /// Download is cancelled
        /// </summary>
        Cancelled,
        /// <summary>
        /// An error occured while downloading
        /// </summary>
        ErrorOccured
    }
}
