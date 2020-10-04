using System;
using System.IO;
using System.Windows.Forms;
using AltoHttp;

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
            var url = txtUrl.Text;
            var defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var defaultFileName = "default.unknown";
            var defaultSavePath = Path.Combine(defaultFolder, defaultFileName);
            downloader = new HttpDownloader(url, defaultFolder);
            downloader.StatusChanged += downloader_StatusChanged;
            downloader.DownloadInfoReceived += downloader_DownloadInfoReceived;
            downloader.DownloadCompleted += downloader_DownloadCompleted;
            downloader.ProgressChanged += downloader_ProgressChanged;

            btnStart.Enabled = false;
            downloader.Start();;
        }

        void downloader_ProgressChanged(object sender, AltoHttp.ProgressChangedEventArgs e)
        {
            lblTotalBytesReceived.Text = string.Format("{0} / {1}",
                downloader.TotalBytesReceived,
                downloader.Info.Length);
            lblProgress.Text = e.Progress.ToString("0.00") + "%";
            lblSpeed.Text = e.SpeedInBytes.ToHumanReadableSize() + "/s";
        }

        void downloader_DownloadCompleted(object sender, EventArgs e)
        {
            MessageBox.Show("Download comleted!");
            Application.Exit();
        }

        void downloader_DownloadInfoReceived(object sender, EventArgs e)
        {
            var saveDirectory = Path.GetDirectoryName(downloader.FullFileName);
            var serverFileName = downloader.Info.ServerFileName;

            var newFilePath = Path.Combine(saveDirectory, serverFileName);

            //downloader.FullFileName = newFilePath;

            lblFileName.Text = downloader.Info.ServerFileName;
            lblResumeability.Text = downloader.Info.AcceptRange ? "Yes" : "No";
            lblSize.Text = downloader.Info.Length.ToHumanReadableSize();
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
