namespace AppLogApi.Models.AppLog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    public class DBANotificationLogViewModel
    {
        public string ServerName { get; set; }

        public string Title { get; set; }

        public string MsgInfo { get; set; }

        public DateTime LogDate { get; set; }
    }
}
