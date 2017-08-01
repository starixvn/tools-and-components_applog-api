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
        public int LogId { get; set; }

        public string Message { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string AppDomainName { get; set; }

        public string FormattedMessage { get; set; }

        public string Referrer
        {
            get
            {
                return FormattedMessages(this.SplitingMessages, "REFERRER:");
            }
        }

        public string ClientIp
        {
            get
            {
                return FormattedMessages(this.SplitingMessages, "CLIENT IP:");
            }
        }

        public string MachineIP { get; set; }


        public string Machine
        {
            get
            {
                return FormattedMessages(this.SplitingMessages, "Machine:");
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
                return FormattedMessages(this.SplitingMessages, "CUSTID:");
            }
        }

        public string SiteAddress
        {
            get
            {
                return FormattedMessages(this.SplitingMessages, "Url:");
            }
        }

        public string Severity
        {
            get
            {
                return FormattedMessages(this.SplitingMessages, "Severity:");


            }
        }

        public string TheMessage
        {
            get
            {
                return FormattedMessages(this.SplitingMessages, "Message:");
            }
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Message);

        private string[] SplitingMessages
        {
            get
            {
                string[] stringSeparators = new[] { "\r\n" };

                if (!string.IsNullOrWhiteSpace(this.FormattedMessage))
                {
                    return this.FormattedMessage.Split(stringSeparators, StringSplitOptions.None);
                }

                return null;
            }
        }

        private static DateTime ConvertToGMT7(DateTime fromDate)
        {
            var toTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var localTimeZone = TimeZoneInfo.Local;

            if (!localTimeZone.StandardName.Equals(toTimeZone.StandardName))
            {
                DateTime convertedTime = TimeZoneInfo.ConvertTime(fromDate, toTimeZone);

                return convertedTime;
            }

            return fromDate;
        }

        private static string FormattedMessages(string[] messageList, string contentKey)
        {
            if (messageList.Any())
            {
                var fomattedMessage = messageList.FirstOrDefault(item => item.IndexOf(contentKey, StringComparison.OrdinalIgnoreCase) >= 0);

                if (string.IsNullOrWhiteSpace(fomattedMessage))
                {
                    return "N/A";
                }

                return fomattedMessage.Replace(contentKey, string.Empty);
            }

            return string.Empty;
        }
    }
}