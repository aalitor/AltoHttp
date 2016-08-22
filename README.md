# AltoHttp
This simple library provides downloading over Http. It supports Pause and Resume in both for download and download queue

•	Download file with just one line of code

•	Get most useful properties of the download source such as ContentLength, Accept-Range..

•	Due to event based programming, managing downloads is so easy

•	Reports progress and speed every time when the progress is changed

•	Create download queue with DownloadQueue and manage your download list

#Usage
Downloading is so simple
<pre>
<code>
HttpDownloader downloader = new HttpDownloader(targetUrl,targetPath);
downloader.Start(); 
//PAUSE
downloader.Pause();
//RESUME
downloader.Resume();
</code>
</pre>

Queue using
<pre><code>
DownloadQueue myQueue = new DownloadQueue();
myQueue.Add(targetUrl1,targetPath1);
myQueue.Add(targetUrl2,targetPath2);
myQueue.Start();
//PAUSE
myQueue.Pause();
//RESUME
myQueue.Resume();
</code></pre>


