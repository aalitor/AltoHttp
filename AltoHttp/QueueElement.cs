using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp
{
    struct QueueElement
    {
        public override bool Equals(object obj)
        {
            if (obj is QueueElement)
            {
                QueueElement el = (QueueElement)obj;
                return el.Url == Url && el.Destination == Destination && el.Completed == Completed && el.Id == Id;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Url.GetHashCode() ^ Destination.GetHashCode() ^ Completed.GetHashCode();
        }
        public string Id;
        public string Url;
        public string Destination;
        public bool Completed;
    }
}
