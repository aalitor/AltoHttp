using System;
using System.Windows.Forms;
using AltoHttp.NativeMessages;
namespace DemoApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MSG = Receiver.ReadDownloadMessage();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AltoHttpDemoForm());
        }
        public static DownloadMessage MSG = null;
    }
}
