using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Letstalk.Bots.AppLogSender.Models
{
    /// <summary>
    /// should be flat objects
    /// </summary>
    public class LogEntry
    {
        private readonly string[] NewLineSeperator = new[] { "\r\n" };
        private const string AsiaStandardTime = "SE Asia Standard Time";
        private const string NotAvaiable = "N/A";

        public int LogId { get; set; }

        public string Message { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string AppDomainName { get; set; }

        public string FormattedMessage { get; set; }

        public string MachineIP { get; set; }

        public string Referrer => ExtractValue(this.SplitingMessages, "REFERRER:");

        public string ClientIp => ExtractValue(this.SplitingMessages, "CLIENT IP:");

        public string Machine => ExtractValue(this.SplitingMessages, "Machine:");

        public string CustId => ExtractValue(this.SplitingMessages, "CUSTID:");

        public string SiteAddress => ExtractValue(this.SplitingMessages, "Url:");

        public string Severity => ExtractValue(this.SplitingMessages, "Severity:");

        public string TheMessage => ExtractValue(this.SplitingMessages, "Message:");

        public DateTime Timestamp { get; set; }

        public DateTime LocalTime => ConvertToGMT7(Timestamp, TimeZoneInfo.FindSystemTimeZoneById(AsiaStandardTime));

        public bool IsValid => !string.IsNullOrWhiteSpace(Message);

        private string[] SplitingMessages
            => string.IsNullOrWhiteSpace(this.FormattedMessage) 
            ? new string[] { }
            : this.FormattedMessage.Split(NewLineSeperator, StringSplitOptions.RemoveEmptyEntries);

        private static DateTime ConvertToGMT7(DateTime fromDate, TimeZoneInfo toTimeZone) =>
             TimeZoneInfo.Local.StandardName.Equals(toTimeZone.StandardName)
                ? fromDate
                : TimeZoneInfo.ConvertTime(fromDate, toTimeZone);

        private static string ExtractValue(string[] messageList, string key)
        {
            var fomattedMessage = messageList
               .FirstOrDefault(item => item.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0);

            return string.IsNullOrWhiteSpace(fomattedMessage)
                ? NotAvaiable
                : fomattedMessage.Replace(key, string.Empty);
        }
    }
}