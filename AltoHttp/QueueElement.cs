using System;

namespace AltoHttp
{
    /// <summary>
    /// Struct to store the informations for download operations
    /// </summary>
    public struct QueueElement
    {
        /// <summary>
        /// Checks if two objects are the same
        /// </summary>
        /// <param name="obj">Target object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is QueueElement)
            {
                QueueElement el = (QueueElement)obj;
                return el.Url == Url && el.Destination == Destination && el.Completed == Completed && el.Id == Id;
            }
            return false;
        }
        /// <summary>
        /// Get the unique hash code of the object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            
            return Id.GetHashCode() ^ Url.GetHashCode() ^ Destination.GetHashCode() ^ Completed.GetHashCode();
        }
		/// <summary>
		/// Download object id
		/// </summary>
		public string Id{ get; set; }
        /// <summary>
        /// Url source string
        /// </summary>
        public string Url{ get; set; }
        /// <summary>
        /// Destination file path to save the data
        /// </summary>
        public string Destination{ get; set; }
        /// <summary>
        /// Boolean value specifies if the download is completed
        /// </summary>
        public bool Completed{ get; set; }
    }
}
