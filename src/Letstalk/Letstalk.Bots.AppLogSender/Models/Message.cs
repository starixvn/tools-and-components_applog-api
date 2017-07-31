using System;
using System.Collections.Generic;

namespace Letstalk.Bots.AppLogSender.Models
{
    public interface IMessage
    {
        int LogId { get; }

        Dictionary<string, string> Content { get; }

        string Category { get; }      
    }

    public class Message : IMessage, IEquatable<Message>
    {
        private readonly LogEntry logEntry;

        public Message(LogEntry logEntry)
        {
            if (logEntry == null)
            {
                throw new ArgumentNullException(nameof(logEntry));
            }

            this.logEntry = logEntry;
        }

        public int LogId => logEntry.LogId;

        public string Category => logEntry.CategoryName;

        public Dictionary<string, string> Content
                => new Dictionary<string, string>
                {
                    ["category"] = logEntry.CategoryName,
                    ["timestamp"] = $"{logEntry.Timestamp.ToShortTimeString()} (GMT-4)",
                    ["localtime"] = $"{logEntry.LocalTime.ToShortTimeString()} (GMT+7)",
                    ["message"] = logEntry.FormattedMessage,
                    ["url"] = logEntry.SiteAddress,
                    ["referrer"] = logEntry.Referrer,
                    ["domain"] = logEntry.AppDomainName,
                    ["machine"] = logEntry.Machine,
                    ["clientip"] = logEntry.ClientIp,
                    ["custid"] = logEntry.CustId,
                    ["severity"] = logEntry.Severity
                };

        public override bool Equals(object obj)
            => (obj is Message) && Equals(obj as Message);     

        public bool Equals(Message other) 
            => this.LogId == other.LogId;

        public override int GetHashCode() 
            => this.logEntry.GetHashCode();
    }
}