using System;
using System.Collections.Generic;

namespace Letstalk.Bots.AppLogSender.Models
{
    public interface IContent
    {
        Dictionary<string, string> Message { get; }

        string Group { get; }

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

        public string Group { get; }

        public Dictionary<string, string> Message
        {
            get => new Dictionary<string, string>
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
                ["custid"] = logEntry.CustId.ToString()
            };
        }

        public List<string> Attachments { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<string> InlineImages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}