using System;
using System.Configuration;
using System.IO;

namespace Letstalk.Bots.AppLogSender.Services
{
    public static class AppSettings
    {
        public static bool IsProduction => Convert.ToBoolean(GetSettingValue("isProduction"));

        public static int Interval => Convert.ToInt32(GetSettingValue("interval"));

        public static string AuthKey => GetSettingValue("letstalk:AuthKey");        

        public static string[] ServiceUri => GetSettingValue("letstalk:Service")
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        
        public static string CategoryGroupMapperFilePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetSettingValue("bot:CategoryGroupMapperPath"));

        public static string IgnoreMessagesFilePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetSettingValue("bot:SkipTheseMessagesPath"));              

        /// <summary>
        /// This function must be called whenever application start
        /// </summary>
        public static void Verify()
        {
            if (string.IsNullOrWhiteSpace(AuthKey))
            {
                throw new InvalidDataException(nameof(AuthKey));
            }

            if (ServiceUri.Length == 0)
            {
                throw new InvalidDataException(nameof(ServiceUri));
            }

            if (!File.Exists(CategoryGroupMapperFilePath))
            {
                throw new FileNotFoundException(CategoryGroupMapperFilePath);
            }

            if (!File.Exists(IgnoreMessagesFilePath))
            {
                throw new FileNotFoundException(IgnoreMessagesFilePath);
            }
        }

        public static string GetSettingValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key));
            }

            return ConfigurationManager.AppSettings[key];
        }
    }
}