using System;

namespace AltoHttp
{
    internal interface IQueue
    {
        event EventHandler QueueCompleted;
        event QueueElementCompletedEventHandler QueueElementCompleted;
        int QueueLength { get; }
        int CurrentIndex { get; }
        void Add(string url, string destPath);
        void StartAsync();
        void ResumeAsync();
        void Pause();
        void Cancel();
    }
}
