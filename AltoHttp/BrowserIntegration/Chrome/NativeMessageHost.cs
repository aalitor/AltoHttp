using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltoHttp.BrowserIntegration.Chrome
{
    class NativeMessageHost
    {
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string[] allowed_origins { get; set; }
        public string path { get; set; }
    }
}
