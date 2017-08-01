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
            var fromTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
            var localTimeZone = TimeZone.CurrentTimeZone;

            if (localTimeZone.StandardName.Equals(fromTimeZone.StandardName))
            {
                DateTime convertedTime = TimeZoneInfo.ConvertTime(fromDate, fromTimeZone, toTimeZone);

                return convertedTime;
            }

            return fromDate;
        }

        //private static string GetContentFromMessage(string message, string contentKey)
        //{
        //    string resultContent = string.Empty;
        //    int startIndex = 0;
        //    int endIndex = 0;
        //    int length = 0;

        //    int contentIndex = message.IndexOf(contentKey, System.StringComparison.InvariantCultureIgnoreCase);
        //    if (contentIndex != -1)
        //    {
        //        startIndex = contentIndex + contentKey.Length;
        //        endIndex = message.IndexOf(Environment.NewLine, startIndex, System.StringComparison.InvariantCultureIgnoreCase);

        //        if (endIndex == -1 && startIndex == contentIndex)
        //        {
        //            endIndex = message.IndexOf(".", startIndex, System.StringComparison.InvariantCultureIgnoreCase);

        //            if (endIndex == -1)
        //            {
        //                resultContent = message;
        //            }
        //        }

        //        if (endIndex != -1)
        //        {
        //            length = endIndex - startIndex;
        //            resultContent = message.Substring(startIndex, length);
        //        }
        //    }

        //    return resultContent;
        //}


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