using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AltoHttp
{
    public delegate void ProgressChangedEventHandler(object sender, DownloadProgressChangedEventArgs e);

    public class HTTPDownloader : IDownloader
    {
        #region Değişkenler
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

        #region Özellikler
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

        #region Yapıcı Metot ve İndirme Prosedürü Metodu
        public HTTPDownloader(string url, string destPath)
        {
            this.Reset();
            fileURL = url;
            this.destPath = destPath;
            oprtor = AsyncOperationManager.CreateOperation(null);
        }
        ~HTTPDownloader()
        {
            this.Cancel();
        }
        void Download(int offset, bool overWriteFile)
        {

            #region İstek Gönder ve Cevap Al
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
            #region Veriyi Al ve Dosyaya Yaz
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

        #region Başlatma,Durdurma,Devam Ettirme, İptal Etme Metotları

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

        public void Pause()
        {
            if (!acceptRange)
                throw new Exception("This download process cannot be paused because it doesn't support ranges");
            if (State == DownloadState.Downloading)
                state = DownloadState.Paused;
        }

        public void ResumeAsync()
        {
            if (State != DownloadState.Paused) return;
            state = DownloadState.Started;
            Task.Run(() =>
            {
                Download((int)bytesReceived, true);
            });
        }

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

        #region Diğer Metotlar
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

    public static class MemoryUnitConverter
    {
        public static string ConvertBestScaledSize(this long bytes)
        {
            int unit = 1024;
            bool inBytes = bytes < unit;
            bool inKb = bytes < unit * unit;
            bool inMb = bytes < unit * unit * unit;
            if (inBytes)
            {
                return bytes + " bytes";
            }
            if (inKb)
            {
                return (bytes / 1024d).ToString("0.00") + " kb";
            }
            if (inMb)
            {
                return (bytes / 1024d / 1024).ToString("0.00") + " mb";
            }
            return (bytes / 1024d / 1024 / 1024).ToString("0.00") + " gb";
        }
        public static double ConvertMemorySize(this long size, FromTo fromTo)
        {
            int degree = (int)fromTo;
            if (degree < 0)
            {
                double divide = Math.Pow(1024, -degree);
                return size / divide;
            }
            else
            {
                double multiplier = Math.Pow(1024, degree);
                return size * multiplier;
            }
        }

    }
    public enum FromTo
    {
        BytesToKb = -1,
        BytesToMb = -2,
        BytesToGb = -3,
        KbToBytes = 1,
        KbToMb = -1,
        KbToGb = -2,
        MbToBytes = 2,
        MbToKb = 1,
        MbToGb = -1,
        GbToBytes = 3,
        GbToKb = 2,
        GbToMb = 1
    }
}
