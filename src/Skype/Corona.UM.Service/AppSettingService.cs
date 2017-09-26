using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.UM.Service
{
   public static class AppSettingService
    {
        public static string BotAPIBaseUrl => ConfigurationManager.AppSettings["BotApi.BaseUrl"];

        public static string BotAPIResource => ConfigurationManager.AppSettings["BotApi.Resource"];

        public static int Interval => Convert.ToInt16(ConfigurationManager.AppSettings["BotApi.Interval"]);

        public static int TimeZone => Convert.ToInt16(ConfigurationManager.AppSettings["BotApi.TimeZone"]);

        public static string  UMServiceUrl => ConfigurationManager.AppSettings["UMServiceURL"];
    }
}
