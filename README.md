# AltoHttp
This simple library provides downloading over Http. It supports Pause and Resume in both for download and download queue

•	Download file with just one line of code

•	Get most useful properties of the download source such as ContentLength, Accept-Range..

•	Due to event based programming, managing downloads is so easy

•	Reports progress and speed every time when the progress is changed

•	Create download queue with DownloadQueue and manage your download list

<h2>Nuget</h2>
<a href="https://www.nuget.org/packages/AltoHttp">Get AltoHttp at Nuget</a>
<pre><code style="font-size:19px;"><b>Install-Package AltoHttp</b></code></pre>


#Usage
Downloading is so simple
<pre>
<code>
HttpDownloader downloader = new HttpDownloader(targetUrl,targetPath);
downloader.Start(); 
//PAUSE
downloader.Pause();
//RESUME
downloader.Resume(); //downloader.Resume(filePathToResume) resumes from the existing file
</code>
</pre>

Queue using
<pre><code>
DownloadQueue myQueue = new DownloadQueue();
myQueue.Add(targetUrl1,targetPath1);
myQueue.Add(targetUrl2,targetPath2);
myQueue.StartAsync();
//PAUSE
myQueue.Pause();
//RESUME
myQueue.ResumeAsync();
</code></pre>

<h3>Demo Application</h3>
<img src="http://i.imgur.com/PokHWEf.png" />


