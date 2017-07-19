using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Letstalk.Bots.AppLogSender.Services
{
    public static class AppSettingService
    {
        public static bool IsProduction
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]);
            }
        }

        public static int Interval
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["interval"]);
            }
        }

        public static string NotificationServer
        {
            get
            {
                return ConfigurationManager.AppSettings["notificationServers"];
            }
        }

        public static string NotificationAuthKey
        {
            get
            {
                return ConfigurationManager.AppSettings["notificationAuthKey"];
            }
        }
    }
}
