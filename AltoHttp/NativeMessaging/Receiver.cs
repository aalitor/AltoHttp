using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AltoHttp.NativeMessages
{
    /// <summary>
    /// Provides methods to get native message from external application
    /// </summary>
    public static class Receiver
    {
        /// <summary>
        /// Extracts text message from native message
        /// </summary>
        /// <param name="data">The native message data</param>
        /// <returns></returns>
        public static string ReadMessageAsString(JObject data)
        {
            if (data == null)
                return null;
            return (data["text"] ?? string.Empty).Value<string>();
        }
        /// <summary>
        /// Reads the native message using stdio
        /// </summary>
        /// <returns></returns>
        public static JObject ReadConsoleInput()
        {
            var stdin = Console.OpenStandardInput();
            var length = 0;

            var lengthBytes = new byte[4];
            stdin.Read(lengthBytes, 0, 4);
            length = BitConverter.ToInt32(lengthBytes, 0);

            var buffer = new char[length];
            using (var reader = new StreamReader(stdin))
            {
                reader.Read(buffer, 0, buffer.Length);
            }


            return (JObject)JsonConvert.DeserializeObject<JObject>(new string(buffer));
        }
        /// <summary>
        /// Gets the native message as parsed to DownloadMessage
        /// </summary>
        /// <returns></returns>
        public static DownloadMessage ReadDownloadMessage()
        {
            var jobj = ReadConsoleInput();
            if (jobj == null)
                return null;
            var prc = ReadMessageAsString(jobj);
            if (prc == null)
                return null;
            return JsonConvert.DeserializeObject<DownloadMessage>(prc);
        }
    }
    
}
