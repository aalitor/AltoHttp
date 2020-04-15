using System;
using System.Collections.Generic;
using System.Threading;

namespace AltoHttp
{
    /// <summary>
    /// Passes the QueueElementCompleted event args
    /// </summary>
    /// <param name="sender">The objects which the event is occured in</param>
    /// <param name="e">Event arguments</param>
    public delegate void QueueElementCompletedEventHandler(object sender, QueueElementCompletedEventArgs e);

    /// <summary>
    /// Provides methods to create and process download queue
    /// </summary>
    public class DownloadQueue : IQueue, IDisposable
    {
        #region Variables

        HttpDownloader downloader;
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
        /// <summary>
        /// Occurs when the queue has been started
        /// </summary>
        public event EventHandler QueueElementStartedDownloading;

        #endregion

        #region Constructor + Destructor
        /// <summary>
        /// Creates a queue and initializes resources
        /// </summary>
        public DownloadQueue()
        {
            downloader = null;
			elements = new List<QueueElement>();
            downloadSpeed = 0;
            queuePaused = true;
        }
        /// <summary>
        /// Destructor for the object
        /// </summary>
        ~DownloadQueue()
        {
            this.Cancel();
        }
        #endregion

        #region Events

        void downloader_DownloadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        	progress = (int)e.Progress;
            this.CurrentProgress = progress;
            downloadSpeed = e.SpeedInBytes;
        }

        void downloader_DownloadCompleted(object sender, EventArgs e)
        {
            if (QueueElementCompleted != null)
                QueueElementCompleted(this, new QueueElementCompletedEventArgs(this.CurrentIndex, currentElement));
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

        #region Properties
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
        /// <summary>
        /// Gets the Range header value of the current download
        /// </summary>
        public bool CurrentAcceptRange
        {
            get { return downloader.Info.AcceptRange; }
        }

        /// <summary>
        /// Gets the State of the current download
        /// </summary>

        #endregion

        #region Methods
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
            if (elements[index].Equals(currentElement) && downloader != null)
            {
                downloader.Pause();
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
            if (downloader != null)
                downloader.Pause();
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
            downloader.Resume();
            queuePaused = false;
        }
        public void ResumeAsync(string filePath)
        {
            if (currentElement.Url == "")
            {
                createNextDownload();
                return;
            }
            downloader.Resume(filePath);
            queuePaused = false;
        }
        /// <summary>
        /// The queue process pauses
        /// </summary>
        public void Pause()
        {
            downloader.Pause();
            queuePaused = true;
        }
        /// <summary>
        /// Removes all resources used
        /// </summary>
        public void Dispose()
        {
            this.Cancel();
        }
        #endregion

        #region Helper Methods
        void createNextDownload()
        {
            QueueElement elt = getFirstNotCompletedElement();
            if (string.IsNullOrEmpty(elt.Url)) return;
            downloader = new HttpDownloader(elt.Url, elt.Destination);
            downloader.DownloadCompleted += downloader_DownloadCompleted;
            downloader.ProgressChanged += downloader_DownloadProgressChanged;
            downloader.Start();
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
