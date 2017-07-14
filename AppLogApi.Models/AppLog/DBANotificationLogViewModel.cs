namespace AppLogApi.Models.AppLog
{
    using System;

    public class DBANotificationLogViewModel
    {
        public string ServerName { get; set; }

        public string Title { get; set; }

        public string MsgInfo { get; set; }

        public DateTime LogDate { get; set; }

        public string TargetName { get; set; }
    }
}