namespace Letstalk.Bots.DatabaseLogSender.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LogEntry
    {
        public string ServerName { get; set; }

        public string Title { get; set; }

        public string MsgInfo { get; set; }

        public DateTime LogDate { get; set; }

        public string TargetName { get; set; }
    }
}
