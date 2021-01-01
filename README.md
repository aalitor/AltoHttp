# AltoHttp
This simple library provides downloading over Http. It supports Pause and Resume in both for download and download queue

•	Download file with just one line of code

•	Get most useful properties of the download source such as ContentLength, Resumeability, ServerFileName..

•	Due to event based programming, managing downloads is so easy

•	Reports progress and speed every time when the progress is changed

•	Create download queue with DownloadQueue and manage your download list

<h2>Nuget</h2>
<a href="https://www.nuget.org/packages/AltoHttp">Get AltoHttp at Nuget</a>
<pre><code style="font-size:19px;"><b>Install-Package AltoHttp</b></code></pre>


<h2>#Usage</h2>
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

<h1>New Features:</h1>
•	Ability to edit Http request and response with events

•	Using DownloadInfoReceived event you can ensure the headers (Content-Length, Resumeability, FileName) are received

<h1>New Features on 01.01.2021</h1>
•	Resume capability improved to be able to resume on some sites that uses chunked stream e.g Google Drive

•	Native messaging methods added for who wants to integrate with browser using extension

•	To prevent resume on file corruption, file validation based on MD5 added

•	New exceptions defined to be able to handle exceptions in UI

•	Download in some sites which Content-Length returns -1 (Unknown) succesfully handled

• DemoApplication changed and improved


<h3>Demo Application</h3>
<img src="https://i.imgur.com/7rytDU2.png" />


