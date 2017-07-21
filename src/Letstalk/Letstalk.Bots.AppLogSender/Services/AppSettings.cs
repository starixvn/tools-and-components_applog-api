using System;
using System.Configuration;
using System.IO;

namespace Letstalk.Bots.AppLogSender.Services
{
    public static class AppSettings
    {
        public static bool IsProduction => Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]);

        public static int Interval => Convert.ToInt32(ConfigurationManager.AppSettings["interval"]);

        public static string AuthKey => ConfigurationManager.AppSettings["letstalk:AuthKey"];

        public static string Service => ConfigurationManager.AppSettings["letstalk:Service"];

        public static readonly string CategoryGroupMapperFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations\\CategoryGroupMapper.json");

        public static readonly string IgnoreMessagesFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations\\SkipTheseMessages.txt");

        public static void Verify()
        {
            if (!File.Exists(CategoryGroupMapperFilePath))
            {
                throw new FileNotFoundException(CategoryGroupMapperFilePath);
            }

            if (!File.Exists(IgnoreMessagesFilePath))
            {
                throw new FileNotFoundException(IgnoreMessagesFilePath);
            }
        }
    }
}