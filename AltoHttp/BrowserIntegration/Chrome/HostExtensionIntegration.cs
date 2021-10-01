using Newtonsoft.Json;
using System.IO;

namespace AltoHttp.BrowserIntegration.Chrome
{
    /// <summary>
    /// Provides methods to do integration for host manifest file
    /// </summary>
    class HostExtensionIntegrator
    {
        /// <summary>
        /// Gets the chrome extension id
        /// </summary>
        public string ExtensionId
        {
            get { return Constants.ExtensionId; }
        }
        /// <summary>
        /// Gets the allowed origins extension id in proper format
        /// </summary>
        public string[] AllowedOrigins
        {
            get
            {
                return new[] { string.Format("chrome-extension://{0}/", ExtensionId) };
            }
        }
        /// <summary>
        /// Exe file path to run when a download starts on chrome
        /// </summary>
        public string ExePath { get; set; }
        /// <summary>
        /// Host manifest file folder to save
        /// </summary>
        public string HostPath { get; set; }
        /// <summary>
        /// Creates host content as plain text
        /// </summary>
        /// <param name="extensionName">Chrome extension name</param>
        /// <returns></returns>
        NativeMessageHost CreateHostContent(string extensionName)
        {
            return new NativeMessageHost()
            {
                name = extensionName,
                description = "Download manager extension helper",
                type = "stdio",
                allowed_origins = AllowedOrigins,
                path = ExePath
            };
        }
        /// <summary>
        /// Creates host json content in string format
        /// </summary>
        /// <param name="extensionName">Chrome extension name</param>
        /// <returns></returns>
        string GetHostJsonString(string extensionName)
        {
            return JsonConvert.SerializeObject(CreateHostContent(extensionName), Formatting.Indented);
        }
        /// <summary>
        /// Creates the host manifest file
        /// </summary>
        /// <param name="extensionName">Chrome extension name</param>
        public void CreateHostFile(string extensionName)
        {
            if (File.Exists(HostPath))
                File.Delete(HostPath);

            File.WriteAllText(HostPath, GetHostJsonString(extensionName));
        }
    }

    
}
