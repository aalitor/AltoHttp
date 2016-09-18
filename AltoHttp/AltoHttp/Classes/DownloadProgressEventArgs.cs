using System;

namespace AltoHttp
{
    public class DownloadProgressChangedEventArgs
    {
        int progress;
        int speed;
        public DownloadProgressChangedEventArgs(int progress, int speed)
        {
            this.progress = progress;
            this.speed = speed;
        }
        public int Progress
        {
            get { return progress; }
        }
        public int Speed
        {
            get { return speed; }
        }
    }
}
