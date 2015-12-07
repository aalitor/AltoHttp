# AltoHttp
This simple library provides downloading over Http. It supports Pause and Resume in both for download and download queue

•	Download file with just one line of code

•	Get most useful properties of the download source such as ContentLength, Accept-Range..

•	Easy to manage download processes with pausing, resuming and cancelling

•	Reports progress and speed every time when the progress is changed

•	Create download queue with DownloadQueue and manage your download list

#Usage
Downloading is so simple
<pre>
<code>
HttpDownloader downloader = new HttpDownloader(targetUrl,targetPath);
downloader.Start(); 
</code>
</pre>
Queue using
<pre><code>
DownloadQueue myQueue = new DownloadQueue();
myQueue.Add(targetUrl1,targetPath1);
myQueue.Add(targetUrl2,targetPath2);
myQueue.Start();
</code></pre>
