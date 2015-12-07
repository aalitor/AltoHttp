using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp
{
    /// <summary>
    /// The class that contains informations about DownloadProgressChangedEvent
    /// </summary>
    public class DownloadProgressChangedEventArgs
    {
        int progress;
        int speed;
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="progress">The download rogress that has been reported by the event</param>
        /// <param name="speed">The download speed that has been reported by the event</param>
        public DownloadProgressChangedEventArgs(int progress, int speed)
        {
            this.progress = progress;
            this.speed = speed;
        }
        /// <summary>
        /// Gets the current download progress of the queue element
        /// </summary>
        public int Progress
        {
            get { return progress; }
        }
        /// <summary>
        /// Gets the current speed of the queue element
        /// </summary>
        public int SpeedInBytes
        {
            get { return speed; }
        }
    }
}
