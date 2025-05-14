using System.Configuration;

namespace Letstalk.Bots.DatabaseLogSender
{
    public static class AppSetting
    {
        public static int Interval => int.Parse(ConfigurationManager.AppSettings["interval"]);
        public static string LetstalkAPI => ConfigurationManager.AppSettings["letstalk:Service"];
        public static string LetstalkKey => ConfigurationManager.AppSettings["letstalk:AuthKey"];
        public static string OneBookUMUrl => ConfigurationManager.AppSettings["ONEBook.UM.API.Url"];
        public static string SentryDsn => ConfigurationManager.AppSettings["SentryDsn"];
        public static string SentryEnvironment => ConfigurationManager.AppSettings["SentryEnvironment"];
        public static bool DebugSentryLog => bool.Parse(ConfigurationManager.AppSettings["DebugSentryLog"]);
        public static string MattermostUrl => ConfigurationManager.AppSettings["MattermostUrl"];
        public static string MattermostBotToken => ConfigurationManager.AppSettings["MattermostBotToken"];
    }
} 