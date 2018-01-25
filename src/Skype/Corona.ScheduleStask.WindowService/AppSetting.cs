using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.ScheduleTask.WindowService
{
    public static class AppSetting
    {
        public static string BotAPIBaseUrl => ConfigurationManager.AppSettings["BotApi.BaseUrl"];
        public static string BotAPIData => ConfigurationManager.AppSettings["BotApi.Data"];
        public static string BotAPIResume => ConfigurationManager.AppSettings["BotApi.Resume"];
        public static int Interval => Convert.ToInt16(ConfigurationManager.AppSettings["BotApi.Interval"]);
        //public static int TimeZone => Convert.ToInt16(ConfigurationManager.AppSettings["BotApi.TimeZone"]);
        public static string UMServiceUrl => ConfigurationManager.AppSettings["UMServiceURL"];
        public static bool IsLocal => ConfigurationManager.AppSettings["IsLocal"] == "true" || ConfigurationManager.AppSettings["IsLocal"] == "1";
    }
}
