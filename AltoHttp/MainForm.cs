/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 14.04.2020
 * Time: 20:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
namespace AltoHttp
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			button2.Enabled = button3.Enabled = false;
		}
		HttpDownloader hd;
		void Button1Click(object sender, EventArgs e)
		{
			hd = new HttpDownloader("https://dev-files.blender.org/file/download/tulqdbjvaqfiaub4uvqs/PHID-FILE-e3wy5kon7qfqwp37mfxt/test.rar",
			                                       @"C:\users\kafeinaltor\desktop\test.rar");
			hd.ProgressChanged += hd_ProgressChanged;
			hd.DownloadCompleted += hd_DownloadCompleted;
			hd.StatusChanged+= hd_StatusChanged;
			hd.ErrorOccured += hd_ErrorOccured;
			hd.Start();
		}

		void hd_ErrorOccured(object sender, System.IO.ErrorEventArgs e)
		{
			Debug.WriteLine(e.GetException().Message);
		}
		void hd_StatusChanged(object sender, StatusChangedEventArgs e)
		{
			Debug.WriteLine(e.Status);
			button2.Enabled = e.Status == Status.Downloading;
			button3.Enabled = e.Status == Status.Paused;
		}
		void hd_DownloadCompleted(object sender, EventArgs e)
		{
			
		}
		
		void hd_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//this.Invoke((MethodInvoker) delegate{
			progressBar1.Value = (int)e.Progress;
			lblSpeed.Text = ((long)e.SpeedInBytes).ToHumanReadableSize();
			lblSize.Text = hd.Info.Length.ToHumanReadableSize();
			lblLength.Text = e.TotalBytesReceived.ToHumanReadableSize();
			     //      });
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			hd.Pause();
		}
		void Button3Click(object sender, EventArgs e)
		{
			hd.Resume(hd.FullFileName);
		}
	}
}
