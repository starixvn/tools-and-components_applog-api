using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Letstalk.Bots.DatabaseLogSender.Services
{
    public static class AppSetting
    {
        public static int Interval => Convert.ToInt32(ConfigurationManager.AppSettings["interval"]);

        public static string LetstalkAPI => ConfigurationManager.AppSettings["letstalk:Service"];

        public static string LetstalkKey => ConfigurationManager.AppSettings["letstalk:AuthKey"];

        public static bool IsProduction => ConfigurationManager.AppSettings["isProduction"] == "false";

        public static string CoronaService => ConfigurationManager.AppSettings["corona.Service"];

        public static string CoronaAuthKey => ConfigurationManager.AppSettings["corona.AuthKey"];

        public static string  UnderMaintenanceService => ConfigurationManager.AppSettings["UnderMaintenanceUrl"]; 
    }
}
