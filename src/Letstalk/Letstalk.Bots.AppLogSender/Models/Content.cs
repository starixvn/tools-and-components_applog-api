using System;
using System.Collections.Generic;

namespace Letstalk.Bots.AppLogSender.Models
{
    public interface IContent
    {
        int LogId { get; }

        Dictionary<string, string> Message { get; }

        string Category { get; }

        List<string> Attachments { get; set; }

        List<string> InlineImages { get; set; }
    }

    public class Content : IContent
    {
        private readonly LogEntry logEntry;

        public Content(LogEntry logEntry)
        {
            if (logEntry == null)
            {
                throw new ArgumentNullException(nameof(logEntry));
            }

            this.logEntry = logEntry;
        }

        public int LogId => logEntry.LogId;

        public string Category => logEntry.CategoryName;


        public Dictionary<string, string> Message
                => new Dictionary<string, string>
                {
                    ["category"] = logEntry.CategoryName,
                    ["timestamp"] = $"{logEntry.Timestamp.ToShortTimeString()} (GMT-4)",
                    ["localtime"] = $"{logEntry.LocalTime.ToShortTimeString()} (GMT+7)",
                    ["message"] = logEntry.Message,
                    ["url"] = logEntry.SiteAddress,
                    ["referrer"] = logEntry.Referrer,
                    ["domain"] = logEntry.AppDomainName,
                    ["machine"] = logEntry.Machine,
                    ["clientip"] = logEntry.ClientIp,
                    ["custid"] = logEntry.CustId,
                    ["severity"] = logEntry.Severity
                };



        public List<string> Attachments
        {
            get; set;
        }

        public List<string> InlineImages
        {
            get; set;
        }
    }
}