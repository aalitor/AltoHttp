using System;
using System.IO;
using System.Windows.Forms;
using AltoHttp;
using AltoHttp.Exceptions;
using System.Net;

namespace DemoApplication
{
    public partial class AltoHttpDemoForm : Form
    {
        private HttpDownloader downloader = null;
        public AltoHttpDemoForm()
        {
            InitializeComponent();

            btnStart.Click+=btnStart_Click;
            btnPuaseOrResume.Click += btnPuaseOrResume_Click;
            this.Load += AltoHttpDemoForm_Load;
        }

        void AltoHttpDemoForm_Load(object sender, EventArgs e)
        {
            if (Program.MSG != null)
            {
                var url = Program.MSG.Url;
                txtUrl.Text = url;
                txtUrl.Enabled = false;
                var defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var defaultFileName = "default.unknown";
                var defaultSavePath = Path.Combine(defaultFolder, defaultFileName);
                downloader = new HttpDownloader(url, defaultSavePath);
                downloader.StatusChanged += downloader_StatusChanged;
                downloader.DownloadInfoReceived += downloader_DownloadInfoReceived;
                downloader.DownloadCompleted += downloader_DownloadCompleted;
                downloader.BeforeSendingRequest += downloader_BeforeSendingRequest;
                downloader.ProgressChanged += downloader_ProgressChanged;
                downloader.ErrorOccured += downloader_ErrorOccured;
                btnStart.Enabled = false;
                btnPuaseOrResume.Enabled = false;
                downloader.Start();
            }
        }

        void btnPuaseOrResume_Click(object sender, EventArgs e)
        {
            if(btnPuaseOrResume.Text == "Pause")
                downloader.Pause();
            else
                downloader.Resume();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Program.MSG == null)
            {
                var url = txtUrl.Text;
                var defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var defaultFileName = "default.unknown";
                var defaultSavePath = Path.Combine(defaultFolder, defaultFileName);
                downloader = new HttpDownloader(url, defaultSavePath);
                downloader.StatusChanged += downloader_StatusChanged;
                downloader.DownloadInfoReceived += downloader_DownloadInfoReceived;
                downloader.DownloadCompleted += downloader_DownloadCompleted;
                downloader.BeforeSendingRequest += downloader_BeforeSendingRequest;
                downloader.ProgressChanged += downloader_ProgressChanged;
                downloader.ErrorOccured += downloader_ErrorOccured;
                btnStart.Enabled = false;
                btnPuaseOrResume.Enabled = false;
                downloader.Start();
            }
        }

        void downloader_BeforeSendingRequest(object sender, BeforeSendingRequestEventArgs e)
        {
           if(Program.MSG != null)
           {
               var request = (HttpWebRequest)e.Request;
               request.SetHeaders(Program.MSG.Headers);
           }
        }

        void downloader_ErrorOccured(object sender, ErrorEventArgs e)
        {
            var ex = e.GetException();
            if (ex is FileValidationFailedException)
            {
                downloader.Pause();
            }
            MessageBox.Show("Error: " + e.GetException().Message + " " + e.GetException().StackTrace);
        }

        void downloader_ProgressChanged(object sender, AltoHttp.ProgressChangedEventArgs e)
        {
            lblTotalBytesReceived.Text = string.Format("{0} / {1}",
                e.TotalBytesReceived.ToHumanReadableSize(),
                downloader.Info.Length > 0 ? downloader.Info.Length.ToHumanReadableSize() : "Unknown");
            lblProgress.Text = e.Progress.ToString("0.00") + "%";
            lblSpeed.Text = e.SpeedInBytes.ToHumanReadableSize() + "/s";
            progressBar.Value = (int)(e.Progress * 100);
        }

        void downloader_DownloadCompleted(object sender, EventArgs e)
        {
            MessageBox.Show("Download comleted!");
            Application.Exit();
        }

        void downloader_DownloadInfoReceived(object sender, EventArgs e)
        {
            btnPuaseOrResume.Enabled = downloader.Info.AcceptRange;

            var saveDirectory = Path.GetDirectoryName(downloader.FullFileName);
            var serverFileName = downloader.Info.ServerFileName;

            var newFilePath = Path.Combine(saveDirectory, serverFileName);

            downloader.FullFileName = newFilePath;

            lblFileName.Text = downloader.Info.ServerFileName;
            lblResumeability.Text = downloader.Info.AcceptRange ? "Yes" : "No";
            lblSize.Text = downloader.Info.Length > 0 ? downloader.Info.Length.ToHumanReadableSize() : "Unknown";
            lblIsChunked.Text = downloader.Info.IsChunked ? "Yes" : "No";
        }

        void downloader_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status == Status.Downloading)
            {
                btnPuaseOrResume.Text = "Pause";
            }
            else if (e.Status == Status.Paused)
            {
                btnPuaseOrResume.Text = "Resume";
            }
        }
    }
}
