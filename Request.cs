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
        public int QueueNumber { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
        public int OperationsProcessed { get; set; }
        public bool IsWaitingForResource { get; set; }
    }
}
