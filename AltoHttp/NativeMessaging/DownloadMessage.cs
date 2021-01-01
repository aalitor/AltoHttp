
using System.Net;

namespace AltoHttp.NativeMessages
{
	/// <summary>
	/// Contains download information got from external
	/// </summary>
	public class DownloadMessage
    {
        /// <summary>
        /// Remote file size for download in external application
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// Remote url for download in external application
        /// </summary>
        public string Url { get; set; }

        public string TabUrl { get; set; }
        /// <summary>
        /// Remote filename for download in external application
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Request headers for download in external application
        /// </summary>
        public Header[] Headers { get; set; }
        /// <summary>
        /// Converts headers to header collection
        /// </summary>
        /// <returns></returns>
        public WebHeaderCollection GetWebHeaders()
        {
            var whc = new WebHeaderCollection();
            foreach (var item in Headers)
            {
                whc.Add(item.Name, item.Value);
            }
            return whc;
        }
    }
}
