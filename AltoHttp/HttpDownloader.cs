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
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
namespace AltoHttp
{
	/// <summary>
	/// Description of HttpDownloader.
	/// </summary>
	public class HttpDownloader
	{
		private AsyncOperation aop;
		public event EventHandler<EventArgs> DownloadCompleted;
		public event EventHandler<BeforeSendingRequestEventArgs> BeforeSendingRequest;
		public event EventHandler<AfterGettingResponseEventArgs> AfterGettingResponse;
		public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
		public event EventHandler<StatusChangedEventArgs> StatusChanged;
		public event EventHandler<ErrorEventArgs> ErrorOccured;
		private volatile bool allowDownload;
		private Stopwatch stp;
		private long speedBytesTotal;
		public HttpDownloader(string url, string fullpath)
		{
			this.Url = url;
			this.FullFileName = fullpath;
			allowDownload = true;
			stp = new Stopwatch();
			aop = AsyncOperationManager.CreateOperation(null);
			new GlobalSettings();
		}
		public void GetHeaders()
		{
			Info = RemoteFileInfo.Get(Url, BeforeSendingRequest, AfterGettingResponse);
		}
		void Process()
		{
			try
			{
				if (Info == null)
				{
					State = Status.GettingHeaders;
					GetHeaders();
				}
				var append = RemainingRangeStart > 0;
				int bytesRead = 0;
				var buffer = new byte[2 * 1024];
				State = Status.SendingRequest;
				using (var response = RequestHelper.CreateRequestGetResponse(Info, RemainingRangeStart, BeforeSendingRequest, AfterGettingResponse))
				{
					State = Status.GettingResponse;
					using (var responseStream = response.GetResponseStream())
					using (var file = FileHelper.CheckFile(FullFileName, append))
					{
						while (allowDownload && (bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
						{
							State = Status.Downloading;
							file.Write(buffer, 0, bytesRead);
							TotalBytesReceived += bytesRead;
							speedBytesTotal += bytesRead;
							aop.Post(new System.Threading.SendOrPostCallback(
									delegate
									{
										ProgressChanged.Raise(this,
												new ProgressChangedEventArgs(SpeedInBytes, this.Progress, TotalBytesReceived));
									}), Progress);
						}
					}
				}
				
				if (RemainingRangeStart > Info.Length)
					throw new Exception("Total bytes received overflows content length");
				if (TotalBytesReceived == Info.Length)
				{
					State = Status.Completed;
					DownloadCompleted.Raise(this, EventArgs.Empty);
				}
				else if (!allowDownload)
				{
					State = Status.Paused;
				}
				
			}
			catch (Exception ex)
			{
				State = Status.ErrorOccured;
				aop.Post(new System.Threading.SendOrPostCallback(
						delegate
						{
							ErrorOccured.Raise(this, new ErrorEventArgs(ex));
						}), null);
			}
		}
		/// <summary>
		/// Starts the download async
		/// </summary>
		public void Start()
		{
			allowDownload = true;
			Task.Run(() =>
				{
					Process();
				});
		}
		/// <summary>
		/// Pause download. Resume is enabled unless application open.
		/// </summary>
		public void Pause()
		{
			if (State != Status.Downloading)
				throw new Exception("Pause is enabled only when downloading");
			allowDownload = false;
		}
		/// <summary>
		/// Continues from where the file left
		/// Note that to avoid corrupted files you definitely use validation
		/// </summary>
		/// <param name="fileToResume">The filepath to continue</param>
		public void Resume(string fileToResume)
		{
			if (State != Status.Paused && State != Status.ErrorOccured)
				throw new Exception("Resume is enabled only when not downloading");
			var file = new FileInfo(fileToResume);
			TotalBytesReceived = file.Length;
			if (fileToResume != FullFileName)
				File.Copy(fileToResume, FullFileName);
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
		
		public long TotalBytesReceived{ get; private set; }
		public string FileName
		{ 
			get
			{
				return Path.GetFileName(FullFileName);
			} 
		}
		public string FullFileName
		{
			get;
			private set;
		}
		public string Url{ get; private set; }
		public long RemainingRangeStart
		{
			get
			{
				return TotalBytesReceived;
			}
		}
		private Status state;
		Status State
		{
			get{ return state; }
			set
			{
				if (value != state)
				{
					aop.Post(
						new System.Threading.SendOrPostCallback(delegate
							{
								StatusChanged.Raise(this, new StatusChangedEventArgs(value));
							}), null);
					if (value == Status.Downloading)
					{
						stp.Start();
					}
					else
					{
						speedBytesTotal = 0;
						stp.Stop();
					}
				}
				state = value;
				
			}
		}
		public RemoteFileInfo Info{ get; private set; }
		int SpeedInBytes
		{
			get
			{
				return (int)(speedBytesTotal * 1d / stp.Elapsed.TotalSeconds);
			} 
		}
		double Progress { get { return TotalBytesReceived * 100d / Info.Length; } }
	}
}
