namespace AppLogApi.Models.AppLog
{
    using System;

    public class AppLogViewModel
    {
        public int LogId { get; set; }

        public DateTime Timestamp { get; set; }

        public string Severity { get; set; }

        public string FormattedMessage { get; set; }

        public Category Category { get; set; }

        public Machine Machine { get; set; }

        public GroupChat GroupChat { get; set; }
    }
}
