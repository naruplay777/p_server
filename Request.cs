using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p_server
{
    public class Request
    {
        public string ClientInfo { get; set; }
        public string Content { get; set; }
        public bool HasPaper { get; set; }
        public bool HasPrinter { get; set; }
        public int QueueNumber { get; set; } // Nueva propiedad
    }

}
