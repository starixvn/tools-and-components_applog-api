using System;
using System.Linq;

namespace Letstalk.Bots.AppLogSender.Models
{
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

        public string Referrer => ExtractValue(GetSplitingMessages(), "REFERRER:");

        public string ClientIp => ExtractValue(GetSplitingMessages(), "CLIENT IP:");

        public string Machine => ExtractValue(GetSplitingMessages(), "Machine:");

        public string CustId => ExtractValue(GetSplitingMessages(), "CUSTID:");

        public string SiteAddress => ExtractValue(GetSplitingMessages(), "Url:");

        public string Severity => ExtractValue(GetSplitingMessages(), "Severity:");

        public string TheMessage => ExtractValue(GetSplitingMessages(), "Message:");

        public DateTime Timestamp { get; set; }

        public DateTime LocalTime => ConvertToGMT7(Timestamp, TimeZoneInfo.FindSystemTimeZoneById(AsiaStandardTime));

        public bool IsValid => !string.IsNullOrWhiteSpace(Message);

        private string[] GetSplitingMessages()
        {
            return string.IsNullOrWhiteSpace(FormattedMessage)
            ? new string[] { }
            : FormattedMessage.Split(NewLineSeperator, StringSplitOptions.RemoveEmptyEntries);
        }

        private static DateTime ConvertToGMT7(DateTime fromDate, TimeZoneInfo toTimeZone)
        {
            return TimeZoneInfo.Local.StandardName.Equals(toTimeZone.StandardName)
                           ? fromDate
                           : TimeZoneInfo.ConvertTime(fromDate, toTimeZone);
        }

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