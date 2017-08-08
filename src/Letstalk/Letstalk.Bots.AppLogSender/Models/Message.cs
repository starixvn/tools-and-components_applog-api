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

    public sealed class Message : IMessage, IEqualityComparer<IMessage>, IEquatable<IMessage>
    {
        private readonly LogEntry logEntry;

        public Message(LogEntry logEntry)
        {
            this.logEntry = logEntry ?? throw new ArgumentNullException(nameof(logEntry));
        }

        public int LogId => logEntry.LogId;

        public string Category => logEntry.CategoryName;

        public Dictionary<string, string> Content
                => new Dictionary<string, string>
                {
                    ["category"] = logEntry.CategoryName,
                    ["timestamp"] = $"{logEntry.Timestamp.ToShortTimeString()} (GMT-4)",
                    ["localtime"] = $"{logEntry.LocalTime.ToShortTimeString()} (GMT+7)",
                    ["message"] = logEntry.TheMessage,
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

        public bool Equals(IMessage other)
            => LogId == other.LogId;

        public bool Equals(IMessage x, IMessage y)
         => x.LogId == y.LogId;

        public int GetHashCode(IMessage obj)
         => logEntry.GetHashCode();

        public override int GetHashCode()
        => logEntry.GetHashCode();
    }
}