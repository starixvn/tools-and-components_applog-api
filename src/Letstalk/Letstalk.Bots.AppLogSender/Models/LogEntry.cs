using System;
using System.Collections.Immutable;
using System.Linq;

namespace Letstalk.Bots.AppLogSender.Models
{
    /// <summary>
    /// should be flat objects
    /// </summary>
    public class LogEntry
    {
        public string Message { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string AppDomainName { get; set; }

        public string FormattedMessage { get; set; }

        public string Referrer { get; set; }

        public string ClientIp { get; set; }

        public string MachineIP { get; set; }

        public string Machine { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime LocalTime { get => ConvertToGMT7(Timestamp); }

        public int CustId { get; }

        public string SiteAddress { get; }

        public bool IsValid => !string.IsNullOrWhiteSpace(Message);

        private static DateTime ConvertToGMT7(DateTime fromDate)
        {
            var toTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var fromTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
            DateTime convertedTime = TimeZoneInfo.ConvertTime(fromDate, fromTimeZone, toTimeZone);

            return convertedTime;
        }
    }
}