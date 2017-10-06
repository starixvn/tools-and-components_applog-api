using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.Core
{
    public static class Utilities
    {
        public static DateTime DatetimeConverter(DateTime date)
        {
            var convertedDatetime = date.ToString("MM/dd/yyyy hh:mm tt");

            return Convert.ToDateTime(convertedDatetime);
        }

        /// <summary>
        /// >0 : date1 > date 2
        /// =0 : date1 = date2
        /// <0 : date1 < date2
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static int DatetimeComparer(DateTime date1, DateTime date2)
        {
            var convertedDate1 = DatetimeConverter(date1);
            var convertedDate2 = DatetimeConverter(date2);

            return convertedDate1.CompareTo(convertedDate2);
        }

        public static string RandomMessage(string[] messages)
        {
            var ran = new Random();

            var messageIndex = ran.Next(0, messages.Length);

            return messages[messageIndex];
        }
    }
}
