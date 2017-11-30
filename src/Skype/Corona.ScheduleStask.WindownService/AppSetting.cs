using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.ScheduleTask.WindownService
{
    public static class AppSetting
    {
        public static string BotAPIBasePath => ConfigurationManager.AppSettings["BotApi.BaseUrl"];

        public static string BotAPIData => ConfigurationManager.AppSettings["BotApi.Data"];
        public static string BotAPIResume => ConfigurationManager.AppSettings["BotApi.Resume"];
        public static int Interval => Convert.ToInt16(ConfigurationManager.AppSettings["BotApi.Interval"]);
        public static int TimeZone => Convert.ToInt16(ConfigurationManager.AppSettings["BotApi.TimeZone"]);
        public static string UMServicePath => ConfigurationManager.AppSettings["UMServiceURL"];
        public static bool IsLocal => ConfigurationManager.AppSettings["IsLocal"] == "true" || ConfigurationManager.AppSettings["IsLocal"] == "1";
    }
}
