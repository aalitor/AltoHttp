using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AltoHttp
{
    /// <summary>
    /// ProgressChanged event handler
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event arguments</param>
    public delegate void ProgressChangedEventHandler(object sender, DownloadProgressChangedEventArgs e);
    /// <summary>
    /// Contains methods to help downloading
    /// </summary>
    public class HttpDownloader : IDownloader
    {
        #region Variables
        /// <summary>
        /// Occurs when the download process is completed
        /// </summary>
        public event EventHandler DownloadCompleted;
        /// <summary>
        /// Occurs when the download process is cancelled
        /// </summary>
        public event EventHandler DownloadCancelled;
        /// <summary>
        /// Occurs when the download progress is changed
        /// </summary>
        public event ProgressChangedEventHandler DownloadProgressChanged;
        /// <summary>
        /// Fired when response headers received e.g ContentLegth, Resumeability
        /// </summary>
        public event EventHandler HeadersReceived;
        HttpWebRequest req;
        HttpWebResponse resp;
        Stream str;
        FileStream file;
        Stopwatch stpWatch;
        AsyncOperation oprtor;
        int bytesReceived, progress, speed, speedBytes;
        long contentLength;
        bool acceptRange;
        string fileURL, destPath;
        DownloadState state;
        #endregion

        #region Properties
        /// <summary>
        /// Gets content size of the file
        /// </summary>
        public long ContentSize
        {
            get { return contentLength; }
        }
        /// <summary>
        /// Gets the total bytes count received
        /// </summary>
        public long BytesReceived
        {
            get { return bytesReceived; }
        }
        /// <summary>
        /// Gets the current download speed in bytes
        /// </summary>
        public int SpeedInBytes
        {
            get { return speed; }
        }
        /// <summary>
        /// Gets the current download progress over 100
        /// </summary>
        public int Progress
        {
            get { return progress; }
            private set
            {
                progress = value;
                oprtor.Post(new SendOrPostCallback(delegate
                {
                    if (DownloadProgressChanged != null)
                        DownloadProgressChanged(this, new DownloadProgressChangedEventArgs(progress, speed));
                }), null);

            }
        }
        /// <summary>
        /// Get the source URL that will be downloaded when the download process is started
        /// </summary>
        public string FileURL
        {
            get
            {
                return fileURL;
            }
        }

        /// <summary>
        /// Gets the destination path that the file will be saved when the download process is completed
        /// </summary>
        public string DestPath
        {
            get
            {
                return destPath;
            }
        }
        /// <summary>
        /// Returns true if the source supports to set ranges into the requests, if not returns false
        /// </summary>
        public bool AcceptRange
        {
            get { return acceptRange; }
        }
        /// <summary>
        /// Gets the value that reports the state of the download process
        /// </summary>
        public DownloadState State
        {
            get
            {
                return state;
            }
            private set
            {
                state = value;
                if (state == DownloadState.Completed && DownloadCompleted != null)
                    oprtor.Post(new SendOrPostCallback(delegate
                    {
                        if (DownloadCompleted != null)
                            DownloadCompleted(this, EventArgs.Empty);
                    }), null);
                else if (state == DownloadState.Cancelled && DownloadCancelled != null)
                    oprtor.Post(new SendOrPostCallback(delegate
                    {
                        if (DownloadCancelled != null)
                            DownloadCancelled(this, EventArgs.Empty);
                    }), null);
            }
        }
        #endregion

        #region Constructor, Destructor, Download Procedure
        /// <summary>
        /// Creates an instance of the HttpDownloader class
        /// </summary>
        /// <param name="url">Url source string</param>
        /// <param name="destPath">Target file path</param>
        public HttpDownloader(string url, string destPath)
        {
            this.Reset();
            fileURL = url;
            this.destPath = destPath;
            oprtor = AsyncOperationManager.CreateOperation(null);
        }
        /// <summary>
        /// Destructor of the class object
        /// </summary>
        ~HttpDownloader()
        {
            this.Cancel();
        }
        void Download(int offset, bool overWriteFile)
        {

            #region Send Request, Get Response
            try
            {
                req = WebRequest.Create(this.FileURL) as HttpWebRequest;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362";
                req.ServicePoint.ConnectionLimit = 999;
                req.AddRange(offset);
                req.AllowAutoRedirect = true;
                resp = req.GetResponse() as HttpWebResponse;
                str = resp.GetResponseStream();
                if (!overWriteFile)
                {
                    contentLength = resp.ContentLength;
                    acceptRange = getAcceptRangeHeaderValue();
                    if (HeadersReceived != null)
                        oprtor.Post(new SendOrPostCallback(delegate
                        {
                            HeadersReceived(this, EventArgs.Empty);
                        }), null);
                }
            }
            catch (Exception)
            {
                state = DownloadState.Completed;
                return;
            }
            #endregion

            if (overWriteFile)
                file = File.Open(destPath, FileMode.Append, FileAccess.Write);
            else
            {
                if (File.Exists(destPath))
                    file = new FileStream(destPath, FileMode.Truncate, FileAccess.Write);
                else
                    file = new FileStream(destPath, FileMode.Create, FileAccess.Write);
            }
            int bytesRead = 0;
            speedBytes = 0;
            byte[] buffer = new byte[4096];
            stpWatch.Reset();
            stpWatch.Start();
            
            #region Get the data to the buffer, write it to the file
            while ((bytesRead = str.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (state == DownloadState.Cancelled | state == DownloadState.Paused) break;
                state = DownloadState.Downloading;
                file.Write(buffer, 0, bytesRead);
                file.Flush();
                bytesReceived += bytesRead;
                speedBytes += bytesRead;
                this.Progress = progress = (int)(bytesReceived * 100.0 / contentLength);
                speed = (int)(speedBytes / 1.0 / stpWatch.Elapsed.TotalSeconds);
            }
            #endregion

            stpWatch.Reset();
            CloseResources();
            Thread.Sleep(100);
            if (state == DownloadState.Downloading)
            {
                state = DownloadState.Completed;
                this.State = state;
            }
        }

        #endregion

        #region Start, Pause, Stop, Resume
        /// <summary>
        /// Starts the download async
        /// </summary>
        public async void StartAsync()
        {
            if (state != DownloadState.Started & state != DownloadState.Completed & state != DownloadState.Cancelled)
                return;

            state = DownloadState.Started;
            await Task.Run(() =>
            {
                Download(0, false);
            });

        }
        /// <summary>
        /// Pauses the download process
        /// </summary>
        public void Pause()
        {
            if (!acceptRange)
                throw new Exception("This download process cannot be paused because it doesn't support ranges");
            if (State == DownloadState.Downloading)
                state = DownloadState.Paused;
        }
        /// <summary>
        /// Resumes the download, only if the download is paused
        /// </summary>
        public void ResumeAsync()
        {
            if (State != DownloadState.Paused) return;
            state = DownloadState.Started;
            Task.Run(() =>
            {
                Download((int)bytesReceived, true);
            });
        }
        /// <summary>
        /// Cancels the download and deletes the uncompleted file which is saved to destination
        /// </summary>
        public void Cancel()
        {
            if (state == DownloadState.Completed | state == DownloadState.Cancelled | state == DownloadState.ErrorOccured) return;
            if (state == DownloadState.Paused)
            {
                this.Pause();
                state = DownloadState.Cancelled;
                Thread.Sleep(100);
                CloseResources();
            }

            state = DownloadState.Cancelled;
        }
        #endregion

        #region Helper Methods
        void Reset()
        {
            progress = 0;
            bytesReceived = 0;
            speed = 0;
            stpWatch = new Stopwatch();
        }
        void CloseResources()
        {
            if (resp != null)
                resp.Close();
            if (file != null)
                file.Close();
            if (str != null)
                str.Close();
            if (destPath != null && state == DownloadState.Cancelled | state == DownloadState.ErrorOccured)
            {
                try
                {
                    File.Delete(destPath);
                }
                catch
                {
                    throw new Exception("There is an error unknown. This problem may cause because of the file is in use");
                }
            }
        }
        bool getAcceptRangeHeaderValue()
        {
            for (int i = 0; i < resp.Headers.Count; i++)
            {
                if (resp.Headers.AllKeys[i].Contains("Range"))
                    return resp.Headers[i].Contains("byte");
            }
            return false;
        }
        string getFileNameFromUrl()
        {
            return Path.GetFileName(new Uri(this.fileURL).AbsolutePath);
        }
        #endregion
    }
    
}
