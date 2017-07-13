using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using AppLogApi.Criteria;
using AppLogApi.Models.AppLog;
using AppLogApi.Services.Impls;
using AppLogApi.Services.Interfaces;
using Fanex.Data;
using ONEbook.NotificationService;
using Sunplus.UMControllerLibrary;

namespace Fanex.AutoBotChat.Console
{
    static class Program
    {
        private static IAppLogService _appLogService;
        private static UMBusiness _umService;
        private static int _interval = 1;
        private static string _environmentServers = string.Empty;
        private static string _environmentAuthKey = string.Empty;
        private static bool _isProduction;
        private static string _logPath = string.Empty;
        private static IList<string> Logs;
        static void Main(string[] args)
        {
            DbSettingProviderManager.StartNewSession();
            _appLogService = new AppLogService();
            _umService = new UMBusiness();
            _interval = Convert.ToInt32(ConfigurationManager.AppSettings["interval"]);
            _environmentServers = ConfigurationManager.AppSettings["Environtment-Servers"];
            _environmentAuthKey = ConfigurationManager.AppSettings["Environtment-AuthKey"];
            _isProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"]);
            _logPath = ConfigurationManager.AppSettings["LogPath"];
            Logs = new List<string>();

            Processing();
            WriteLog();            
        }

        public static void WriteLog()
        {
            File.WriteAllLines(_logPath, Logs);
        }

        public static void Processing()
        {
            var criteria = new GetAppLogCriteria
            {
                ClientTimeZone = -4,
                FromDate = DateTime.Now.AddMinutes(-_interval),
                ToDate = DateTime.Now,
                IsProduction = _isProduction,
                Severity = "Error,Critical"
            };

            bool isUnderMaintenance = _umService.isUnderMaintainence(Sites.Agency, ConfigurationManager.AppSettings["UnderMaintenanceUrl"]);

            if (!isUnderMaintenance)
            {
                var appLogs = _appLogService.GetAppLog(criteria).ToList();
                Logs.Add("Get applog data: " + appLogs.Count.ToString());

                var ignoreAppLogsXmlPath = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/IgnoreMessages.xml";

                if (appLogs.Count > 0)
                {
                    string groupChatXmlPath = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/GroupChat.xml";

                    appLogs = _appLogService.MappGroupChatAndGenerateUrlAndReferrer(groupChatXmlPath, appLogs).ToList();
                    Logs.Add("Mapped group chat: " + appLogs.Count.ToString());

                    var appLogsIgnored = _appLogService.GetIgnoreMessage(ignoreAppLogsXmlPath);

                    foreach (var appLog in appLogs)
                    {
                        AssignMessageData(appLog, appLogsIgnored);
                    }
                }
            }
        }

        public static bool SendMessage(string targetName, Dictionary<string, string> vars)
        {
            var serverUrls = _environmentServers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Notification.Initialize(serverUrls, _environmentAuthKey);

            return Notification.Send(targetName, vars);
        }

        private static void AssignMessageData(AppLogViewModel appLog, IList<AppLog> appLogsIgnored)
        {
            if (appLog.GroupChat != null && !IsIgnoreMessage(appLogsIgnored, appLog) && !appLog.GroupChat.IsIgnore)
            {
                var vars = new Dictionary<string, string>
                {
                    { "category", appLog.Category.CategoryName },
                    { "timestamp", appLog.Timestamp.ToShortTimeString() + " (GMT-4)" },
                    { "localtime", ConvertToGMT7(appLog.Timestamp).ToShortTimeString() + " (GMT+7)" },
                    { "severity", appLog.Severity },
                    { "message", appLog.Message },
                    { "url", appLog.SiteAddress },
                    { "referrer", appLog.Referrer },
                    { "domain", appLog.AppDomainName },
                    { "machine", appLog.Machine.MachineIP },
                    { "clientip", appLog.ClientIp },
                    { "custid", appLog.CustId }

                };
                var targets = appLog.GroupChat.Targets.Split(new char[','], StringSplitOptions.RemoveEmptyEntries);

                foreach (var target in targets)
                {
                    SendMessage(target, vars);
                }

                Logs.Add("Message is sent: " + appLog.Message);
            }
        }

        private static bool IsIgnoreMessage(IList<AppLog> ignoreLogs, AppLogViewModel appLog)
        {
            foreach (var ignoreLog in ignoreLogs)
            {
                if (ignoreLog.IsIgnoreMessage(appLog.Message))
                {
                    Logs.Add("Ignored message: " + appLog.Message);
                    return true;
                }
            }

            return false;
        }
        private static DateTime ConvertToGMT7(DateTime fromDate)
        {
            var toTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var fromTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
            DateTime convertedTime = TimeZoneInfo.ConvertTime(fromDate, fromTimeZone, toTimeZone);

            return convertedTime;
        }
    }
}
