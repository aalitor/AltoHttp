using System;
using System.Collections.Generic;
using System.Threading;

namespace AltoHttp
{
    public delegate void QueueElementCompletedEventHandler(object sender, QueueElementCompletedEventArgs e);

    /// <summary>
    /// Provides methods to create and process download queue
    /// </summary>
    public class DownloadQueue : IQueue, IDisposable
    {
        #region Variables

        HTTPDownloader indirici;
        List<QueueElement> elements;
        QueueElement currentElement;
        int progress;
        int downloadSpeed;
        bool queuePaused, startEventRaised;

        /// <summary>
        /// Occurs when queue element's progress is changed
        /// </summary>
        public event EventHandler QueueProgressChanged;
        /// <summary>
        /// Occurs when the queue is completely completed
        /// </summary>
        public event EventHandler QueueCompleted;
        /// <summary>
        /// Occurs when the queue element is completed
        /// </summary>
        public event QueueElementCompletedEventHandler QueueElementCompleted;

        public event EventHandler QueueElementStartedDownloading;

        #endregion

        #region Constructor + Destructor
        /// <summary>
        /// Creates a queue and initializes resources
        /// </summary>
        public DownloadQueue()
        {
            indirici = null;
            elements = new List<QueueElement>();
            downloadSpeed = 0;
            queuePaused = true;
        }
        ~DownloadQueue()
        {
            this.Cancel();
        }
        #endregion

        #region Olaylar

        void indirici_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progress = e.Progress;
            this.CurrentProgress = progress;
            downloadSpeed = e.Speed;
        }

        void indirici_DownloadCompleted(object sender, EventArgs e)
        {
            if (QueueElementCompleted != null)
                QueueElementCompleted(this, new QueueElementCompletedEventArgs(this.CurrentIndex));
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Equals(currentElement))
                {
                    elements[i] = new QueueElement()
                    {
                        Id = elements[i].Id,
                        Url = elements[i].Url,
                        Destination = elements[i].Destination,
                        Completed = true
                    };
                    break;
                }
            }

            createNextDownload();
        }
        #endregion

        #region Özellikler
        /// <summary>
        /// Gets the number of elements in the queue
        /// </summary>
        public int QueueLength
        {
            get { return elements.Count; }
        }
        /// <summary>
        /// Gets the index number of the element that is currently processing
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (!elements[i].Completed) return i;
                }
                return -1;
            }
        }
        /// <summary>
        /// Gets the download progress of the current download process
        /// </summary>
        public int CurrentProgress
        {
            get
            {
                return progress;
            }
            private set
            {
                value = progress;
                if (QueueProgressChanged != null && this.CurrentIndex >= 0 && !queuePaused)
                    QueueProgressChanged(this, EventArgs.Empty);
                if (QueueElementStartedDownloading != null && progress > 0 && !startEventRaised)
                {
                    QueueElementStartedDownloading(this, EventArgs.Empty);
                    startEventRaised = true;
                }
            }
        }
        /// <summary>
        /// Gets the download speed of the current download progress
        /// </summary>
        public int CurrentDownloadSpeed
        {
            get { return downloadSpeed; }
        }
        public bool CurrentAcceptRange
        {
            get { return indirici.AcceptRange; }
        }
        #endregion

        #region Metotlar
        /// <summary>
        /// Adds new download elements into the queue
        /// </summary>
        /// <param name="url">The URL source that contains the object will be downloaded</param>
        /// <param name="destPath">The destination path to save the downloaded file</param>
        public void Add(string url, string destPath)
        {

            elements.Add(new QueueElement()
            {
                Id = Guid.NewGuid().ToString(),
                Url = url,
                Destination = destPath
            });
        }
        /// <summary>
        /// Deletes the queue element at the given index
        /// </summary>
        /// <param name="index">The index of the element that will be deleted</param>
        public void Delete(int index)
        {
            if (elements[index].Equals(currentElement) && indirici != null)
            {
                indirici.Cancel();
                currentElement = new QueueElement() { Url = "" };
            }
            elements.RemoveAt(index);
            if (!queuePaused)
                createNextDownload();
        }
        /// <summary>
        /// Deletes all elements in the queue
        /// </summary>
        public void Clear()
        {
            Cancel();
        }
        /// <summary>
        /// Starts the queue async
        /// </summary>
        public void StartAsync()
        {
            createNextDownload();
        }
        /// <summary>
        /// Stops and deletes all elements in the queue
        /// </summary>
        public void Cancel()
        {
            if (indirici != null)
                indirici.Cancel();
            Thread.Sleep(100);
            elements.Clear();
            queuePaused = true;
        }
        /// <summary>
        /// The queue process resumes
        /// </summary>
        public void ResumeAsync()
        {
            if (currentElement.Url == "")
            {
                createNextDownload();
                return;
            }
            indirici.ResumeAsync();
            queuePaused = false;
        }
        /// <summary>
        /// The queue process pauses
        /// </summary>
        public void Pause()
        {
            indirici.Pause();
            queuePaused = true;
        }
        /// <summary>
        /// Removes all resources used
        /// </summary>
        public void Dispose()
        {
            this.Cancel();
        }
        void createNextDownload()
        {
            QueueElement elt = getFirstNotCompletedElement();
            if (string.IsNullOrEmpty(elt.Url)) return;
            indirici = new HTTPDownloader(elt.Url, elt.Destination);
            indirici.DownloadCompleted += indirici_DownloadCompleted;
            indirici.DownloadProgressChanged += indirici_DownloadProgressChanged;
            indirici.StartAsync();
            currentElement = elt;
            queuePaused = false;
            startEventRaised = false;
        }
        QueueElement getFirstNotCompletedElement()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (!elements[i].Completed) return elements[i];
            }
            if (QueueCompleted != null)
                QueueCompleted(this, new EventArgs());
            return new QueueElement();
        }
        #endregion
    }


}
