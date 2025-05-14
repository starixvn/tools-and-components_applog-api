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

        public static string OneBookUMUrl => ConfigurationManager.AppSettings["ONEBook.UM.API.Url"];

        public static string SentryDsn => ConfigurationManager.AppSettings.Get("SentryDsn");

        public static string SentryEnvironment => ConfigurationManager.AppSettings.Get("SentryEnvironment");

        public static bool DebugSentryLog => bool.Parse(ConfigurationManager.AppSettings.Get("DebugSentryLog"));

        public static string MattermostUrl => ConfigurationManager.AppSettings["MattermostUrl"];

        public static string MattermostBotToken => ConfigurationManager.AppSettings["MattermostBotToken"];
    }
}
