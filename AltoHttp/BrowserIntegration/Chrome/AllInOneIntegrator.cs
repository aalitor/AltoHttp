using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp.BrowserIntegration.Chrome
{
    /// <summary>
    /// Provides methods to add or remove integration on chrome
    /// </summary>
    public class AllInOneIntegrator
    {
        /// <summary>
        /// Tries to integrate chrome return true if successfull, false otherwise
        /// </summary>
        /// <param name="hostFolderToSave">Host manifest folder to save</param>
        /// <param name="exePathToExecute">Executable path of the application</param>
        /// <returns></returns>
        public static bool TryAddIntegration(string hostFolderToSave, string exePathToExecute)
        {
            var isAdmin = RegistryExtensionIntegrator.HasAdminRights();
            if (!isAdmin)
            {
                throw new Exception("You must start the program as admin to integrate. Because registry operations are necessary for integration.");
            }

            var extname = Constants.ExtensionName;
            var extid = Constants.ExtensionId;
            var extensionUrl = Constants.ExtensionUrl;

            var currentDir = Directory.GetCurrentDirectory();
            var hostPath = Path.Combine(hostFolderToSave, extname + ".json");
            var exePath = exePathToExecute;

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appDataChromeExtPath = appData + @"\Google\Chrome\User Data\Default\Extensions\" + extid;

            var h = new HostExtensionIntegrator();
            h.ExePath = exePath;
            h.HostPath = hostPath;
            h.CreateHostFile(extname);

            RegistryExtensionIntegrator.Complete(hostPath, extname);

            var extExists = Directory.Exists(appDataChromeExtPath) && Directory.GetDirectories(appDataChromeExtPath).Any();

            if (RegistryExtensionIntegrator.CheckHost(extname, hostPath) && extExists)
            {
                return true;
            }
            else
            {
                Process.Start(extensionUrl);
                throw new Exception("You must install the chrome extension in browser to complete integration if it is not installed");
            }
        }

        /// <summary>
        /// Tries to remove integration in registry
        /// </summary>
        public static void TryRemoveIntegration()
        {
            RegistryExtensionIntegrator.Remove(Constants.ExtensionName);
        }
    }
}
