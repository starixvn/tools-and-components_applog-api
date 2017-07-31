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
        public int LogId { get; set; }

        public string Message { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string AppDomainName { get; set; }

        public string FormattedMessage {
            get
            {
                var formattedMessage = GetContentFromMessage(this.Message, "Message:");

                return string.IsNullOrWhiteSpace(formattedMessage) ? "N/A" : formattedMessage;
            }
        }

        public string Referrer
        {
            get
            {
                var referrer = GetContentFromMessage(this.Message, "REFERRER:");

                return string.IsNullOrWhiteSpace(referrer) ? "N/A" : referrer;
            }
        }

        public string ClientIp
        {
            get
            {
                var clientIp = GetContentFromMessage(this.Message, "CLIENT IP:");

                return string.IsNullOrWhiteSpace(clientIp) ? "N/A" : clientIp;
            }
        }

        public string MachineIP { get; set; }

        
        public string Machine {
            get
            {
                var machine = GetContentFromMessage(this.Message, "Machine:");

                return string.IsNullOrWhiteSpace(machine) ? "N/A" : machine;
            }
        }

        public DateTime Timestamp { get; set; }

        public DateTime LocalTime
        {
            get
            {
                return ConvertToGMT7(Timestamp);
            }
        }

        public string CustId
        {
            get
            {
                var custId = GetContentFromMessage(this.Message, "CUSTID:");

                return string.IsNullOrWhiteSpace(custId) ? "N/A" : custId;
            }
        }

        public string SiteAddress
        {
            get
            {
                var siteAddress = GetContentFromMessage(this.Message, "Url:");

                return string.IsNullOrWhiteSpace(siteAddress) ? "N/A" : siteAddress;
            }
        }

        public string Severity {
            get
            {
                var severity = GetContentFromMessage(this.Message, "Severity:");

                return string.IsNullOrWhiteSpace(severity) ? "N/A" : severity;
            }
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Message);

        private static DateTime ConvertToGMT7(DateTime fromDate)
        {          
            var toTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var fromTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
            var localTimeZone = TimeZone.CurrentTimeZone;

            if(localTimeZone.StandardName.Equals(fromTimeZone.StandardName))
            {
                DateTime convertedTime = TimeZoneInfo.ConvertTime(fromDate, fromTimeZone, toTimeZone);

                return convertedTime;
            }

            return fromDate;
        }

        private static string GetContentFromMessage(string message, string contentKey)
        {
            string resultContent = string.Empty;
            int startIndex = 0;
            int endIndex = 0;
            int length = 0;

            int contentIndex = message.IndexOf(contentKey, System.StringComparison.InvariantCultureIgnoreCase);
            if (contentIndex != -1)
            {
                startIndex = contentIndex + contentKey.Length;
                endIndex = message.IndexOf(Environment.NewLine, startIndex, System.StringComparison.InvariantCultureIgnoreCase);

                if (endIndex == -1 && startIndex == contentIndex)
                {
                    endIndex = message.IndexOf(".", startIndex, System.StringComparison.InvariantCultureIgnoreCase);

                    if (endIndex == -1)
                    {
                        resultContent = message;
                    }
                }

                if (endIndex != -1)
                {
                    length = endIndex - startIndex;
                    resultContent = message.Substring(startIndex, length);
                }
            }

            return resultContent;
        }
    }
}