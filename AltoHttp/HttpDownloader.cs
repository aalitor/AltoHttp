/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 14.04.2020
 * Time: 20:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using AltoHttp.Exceptions;
namespace AltoHttp
{
    /// <summary>
    /// Description of HttpDownloader.
    /// </summary>
    public class HttpDownloader
    {
        private AsyncOperation aop;
        /// <summary>
        /// Raised when download fully completed
        /// </summary>
        public event EventHandler<EventArgs> DownloadCompleted;
        /// <summary>
        /// Raised before sending request to the remote server
        /// </summary>
        public event EventHandler<BeforeSendingRequestEventArgs> BeforeSendingRequest;
        /// <summary>
        /// Raised after response received from the remote server
        /// </summary>
        public event EventHandler<AfterGettingResponseEventArgs> AfterGettingResponse;
        /// <summary>
        /// Raised when download progress changed 
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        /// <summary>
        /// Raised when download status changed
        /// </summary>
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        /// <summary>
        /// Raised when an error occured in download process
        /// </summary>
        public event EventHandler<ErrorEventArgs> ErrorOccured;
        /// <summary>
        /// Raised when download paused
        /// </summary>
        public event EventHandler<EventArgs> DownloadPaused;
        /// <summary>
        /// Raised when remote file informations received
        /// </summary>
        public event EventHandler<EventArgs> DownloadInfoReceived;
        private volatile bool allowDownload;
        private Stopwatch stp;
        private long speedBytesTotal;
        private Thread downloadThread = null;
        private bool flagResetSpeedBytes = true;
        private Status state;
        /// <summary>
        /// Creates an istance for downloader
        /// </summary>
        /// <param name="url">The remote file source url</param>
        /// <param name="fullpath">The save path to save the downloaded file</param>
        public HttpDownloader(string url, string fullpath)
        {
            this.Url = url;
            this.FullFileName = fullpath;
            allowDownload = true;
            stp = new Stopwatch();
            state = Status.None;
            aop = AsyncOperationManager.CreateOperation(null);
            new GlobalSettings();
        }
        /// <summary>
        /// Gets the response headers
        /// </summary>
        public void GetHeaders()
        {
            Info = RemoteFileInfo.Get(Url, BeforeSendingRequest, AfterGettingResponse);
        }
        private void Process()
        {
            try
            {
                //Get the download headers if not exists
                if (Info == null)
                {
                    State = Status.GettingHeaders;
                    GetHeaders();
                    aop.Post(delegate { DownloadInfoReceived.Raise(this, EventArgs.Empty); }, null);
                }
                //Gets where download left
                var append = RemainingRangeStart > 0;
                var bytesRead = 0;
                var buffer = new byte[2 * 1024];
                State = Status.SendingRequest;

                using (var response = RequestHelper.CreateRequestGetResponse(Info, RemainingRangeStart, BeforeSendingRequest, AfterGettingResponse, Info.IsChunked))
                {

                    State = Status.GettingResponse;
                    using (var responseStream = response.GetResponseStream())
                    using (var file = FileHelper.CheckFile(FullFileName, append, LastMD5Checksum))
                    {
                        while (allowDownload && (bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            State = Status.Downloading;
                            file.Write(buffer, 0, bytesRead);
                            TotalBytesReceived += bytesRead;
                            speedBytesTotal += bytesRead;
                            aop.Post(delegate
                            {
                                ProgressChanged.Raise(this,
                                    new ProgressChangedEventArgs(SpeedInBytes, this.Progress, TotalBytesReceived));
                            }, Progress);
                        }
                    }
                }
                if (Info.Length > 0 && RemainingRangeStart > Info.Length)
                    throw new Exception("Total bytes received overflows content length");
                if (TotalBytesReceived == Info.Length || (Info.Length < 1 && allowDownload))
                {
                    State = Status.Completed;
                    DownloadCompleted.Raise(this, EventArgs.Empty);
                }
                else if (!allowDownload)
                {
                    State = Status.Paused;
                    DownloadPaused.Raise(this, EventArgs.Empty);
                }

            }
            catch (Exception ex)
            {
                if (Info != null && TotalBytesReceived == Info.Length)
                {
                    State = Status.Completed;
                    DownloadCompleted.Raise(this, EventArgs.Empty);
                }
                else if (!allowDownload)
                {
                    State = Status.Paused;
                    DownloadPaused.Raise(this, EventArgs.Empty);
                }
                else
                {
                    LastError = ex;
                    State = Status.ErrorOccured;
                    aop.Post(delegate
                    {
                        ErrorOccured.Raise(this, new ErrorEventArgs(ex));
                    }, null);
                }
            }
            finally
            {
                if (LastError == null || !(LastError is FileValidationFailedException))
                {
                    LastMD5Checksum = FileHelper.CalculateMD5(FullFileName);
                }
            }
        }
        /// <summary>
        /// Starts the download async
        /// </summary>
        public void Start()
        {
            allowDownload = true;
            downloadThread = new Thread(() =>
                {
                    Process();

                    while (allowDownload && TotalBytesReceived != Info.Length && Info.Length > 0 && Info.AcceptRange)
                    {
                        flagResetSpeedBytes = false;
                        Process();
                    }
                    flagResetSpeedBytes = true;
                });
            downloadThread.Start();
        }
        /// <summary>
        /// Pause download. Resume is enabled unless application open.
        /// </summary>
        public void Pause()
        {
            allowDownload = false;
            downloadThread.Abort();
        }
        /// <summary>
        /// Stops the download, deletes the downloaded file and resets the download
        /// </summary>
        public void StopReset()
        {
            Pause();
            if (File.Exists(FullFileName))
                File.Delete(FullFileName);
            stp.Reset();
            speedBytesTotal = 0;
            downloadThread = null;
            flagResetSpeedBytes = true;
            state = Status.None;
        }
        /// <summary>
        /// Continues from where the file left
        /// Note that to avoid corrupted files you definitely use validation
        /// </summary>
        /// <param name="fileToResume">The filepath to continue</param>
        public void Resume(string fileToResume, string validationChecksum)
        {
            LastMD5Checksum = validationChecksum;
            if (State != Status.Paused && State != Status.ErrorOccured)
                throw new Exception("Resume is enabled only when not downloading");
            var file = new FileInfo(fileToResume);
            TotalBytesReceived = file.Length;
            if (fileToResume != FullFileName)
                File.Move(fileToResume, FullFileName);
            Start();
        }
        /// <summary>
        /// Continues from where the file left
        /// </summary>
        public void Resume()
        {
            if (State != Status.Paused && State != Status.ErrorOccured)
                throw new Exception("Resume is enabled only when not downloading");
            Start();
        }
        /// <summary>
        /// Gets the total bytes downloaded
        /// </summary>
        public long TotalBytesReceived
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the filename with extension only
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileName(FullFileName);
            }
        }
        /// <summary>
        /// Gets or sets the file path where the downloaded file will be saved
        /// </summary>
        public string FullFileName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the remote file source url
        /// </summary>
        public string Url
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the byte offset where the download interrupted
        /// </summary>
        public long RemainingRangeStart
        {
            get
            {
                return TotalBytesReceived;
            }
        }
        /// <summary>
        /// Gets the Status of the download
        /// </summary>
        public Status State
        {
            get { return state; }
            private set
            {
                if (value != state)
                {
                    aop.Post(
                        delegate
                        {
                            StatusChanged.Raise(this, new StatusChangedEventArgs(value));
                        }, null);
                    if (value == Status.Downloading)
                    {
                        if (!stp.IsRunning)
                            stp.Start();
                    }
                    else
                    {
                        if (flagResetSpeedBytes)
                        {
                            speedBytesTotal = 0;
                            stp.Reset();
                        }
                    }
                }
                state = value;

            }
        }
        /// <summary>
        /// Gets the remote file properties such as Content-Length, Resumeability...
        /// </summary>
        public RemoteFileInfo Info
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the download speed in bytes
        /// </summary>
        private int SpeedInBytes
        {
            get
            {
                if (stp.Elapsed.TotalSeconds < 1) return 0;
                return (int)(speedBytesTotal * 1d / stp.Elapsed.TotalSeconds);
            }
        }
        /// <summary>
        /// Progress with two decimal places, returns 0 if Content-Length is -1 (unknown)
        /// </summary>
        private double Progress
        {
            get
            {
                if (Info.Length < 1)
                    return 0;
                return TotalBytesReceived * 100d / Info.Length;
            }
        }
        /// <summary>
        /// Gets the last exception occured
        /// </summary>
        public Exception LastError { get; set; }
        /// <summary>
        /// Gets the last calculated md5 hash string
        /// </summary>
        public string LastMD5Checksum { get; set; }

    }
}
