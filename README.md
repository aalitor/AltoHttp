# AltoHttp
AltoHttp provides fast and easy download management

Download is so easy with AltoHttp. It supports pause and resume (especially resuming from file even application closed) and queueing downloads.
Reporting download is also so user friendly with event based programming. You can get the Status info in every step of a download progress.
Event parameters contains speed and progress percentage.
<!-- HTML generated using hilite.me --><div style="background: #ffffff; overflow:auto;width:auto;border:solid gray;border-width:.1em .1em .1em .8em;padding:.2em .6em;"><pre style="margin: 0; line-height: 125%">HttpDownloader hd <span style="color: #333333">=</span> new HttpDownloader(url, savepath);
hd<span style="color: #333333">.</span>Start();
hd<span style="color: #333333">.</span>Pause();
hd<span style="color: #333333">.</span>Resume(); <span style="color: #333333">//</span><span style="color: #000000; font-weight: bold">or</span> hd<span style="color: #333333">.</span>Resume(filetoResume);
</pre></div>


<h2> What's next: </h2>
<ul>
<li>Adding header values and cookies</li>
<li>Adding network credentials</li>
</ul>
