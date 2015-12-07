using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp
{
    /// <summary>
    /// Contains the possible download states
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
        /// Download is processing
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
        /// An error occured while trying to download the file
        /// </summary>
        ErrorOccured
    }
}
