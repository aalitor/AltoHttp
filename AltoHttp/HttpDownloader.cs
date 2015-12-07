using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AltoHttp
{
    /// <summary>
    /// Occurs when the download progress is changed
    /// </summary>
    /// <param name="sender">The object that is associated with the event</param>
    /// <param name="e">Event arguments that contains informations about the event</param>
    public delegate void ProgressChangedEventHandler(object sender, DownloadProgressChangedEventArgs e);
    /// <summary>
    /// Provides methods to download using HTTP
    /// </summary>
    public class HTTPDownloader : IDownloader
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

        HttpWebRequest req;
        HttpWebResponse resp;
        Stream str;
        FileStream file;
        Stopwatch stpWatch;
        AsyncOperation oprtor;
        int bytesReceived;
        int progress;
        int speed;
        int speedBytes;
        long contentLength;
        bool acceptRange;
        string fileURL;
        string destPath;
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
        public int SpeedInByte
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
                oprtor.Post(new SendOrPostCallback(delegate
                {
                    if (DownloadProgressChanged != null && value != progress)
                        DownloadProgressChanged(this, new DownloadProgressChangedEventArgs(value, speed));
                }), null);
                progress = value;
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

        #region Constructor + Destructor + Download Procedure
        /// <summary>
        /// Creates downloader object
        /// </summary>
        /// <param name="url">The source url</param>
        /// <param name="destPath">The destination path that file will be saved</param>
        public HTTPDownloader(string url, string destPath)
        {
            this.Reset();
            fileURL = url;
            this.destPath = destPath;
            oprtor = AsyncOperationManager.CreateOperation(null);
        }
        /// <summary>
        /// Destructor for the download object
        /// </summary>
        ~HTTPDownloader()
        {
            this.Cancel();
        }
        void Download(int offset, bool overWriteFile)
        {

            #region Send Request - Get Response
            try
            {
                req = WebRequest.Create(this.FileURL) as HttpWebRequest;
                req.AddRange(offset);
                req.AllowAutoRedirect = true;
                resp = req.GetResponse() as HttpWebResponse;
                str = resp.GetResponseStream();
                if (!overWriteFile)
                {
                    contentLength = resp.ContentLength;
                    acceptRange = getAcceptRangeHeaderValue();
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
            stpWatch.Start();
            AsyncOperation opsr = AsyncOperationManager.CreateOperation(null);

            #region Get Data - Write To File
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

        #region Start + Pause + Resume + Cancel Methods
        /// <summary>
        /// Starts the download process
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
        /// Resumes paused download
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
        /// Cancels the download process
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

        #region Other Methods Metotlar
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
        #endregion

    }

}
