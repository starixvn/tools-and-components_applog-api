using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppLogApi.Criteria;
using AppLogApi.Models.AppLog;
using AppLogApi.Services.Impls;
using AppLogApi.Services.Interfaces;
using Fanex.Data.Configuration;
using Newtonsoft.Json;
using ONEbook.NotificationService;
using RSACryptoPad;
using Sunplus.UMControllerLibrary;

namespace Fanex.AutoBotChat.Console
{
    static class Program
    {
        private static IAppLogService _appLogService;
        private static UMBusiness _umService;
        private static int _interval;
        private static string _environmentServers = string.Empty;
        private static string _environmentAuthKey = string.Empty;
        static void Main(string[] args)
        {
            DbSettingProviderManager.Current.Start();
            _appLogService = new AppLogService();
            _umService = new UMBusiness();
            _interval = Convert.ToInt32(ConfigurationManager.AppSettings["interval"]);
            _environmentServers = ConfigurationManager.AppSettings["Environtment-Servers"];
            _environmentAuthKey = ConfigurationManager.AppSettings["Environtment-AuthKey"];

            Processing();
        }

        public static void Processing()
        {
            var criteria = new GetAppLogCriteria
            {
                ClientTimeZone = -4,
                FromDate = DateTime.Now.AddMinutes(-_interval),
                ToDate = DateTime.Now,
                IsProduction = true,
                Severity = "Error,Critical"
            };

            bool isUnderMaintenance = _umService.isUnderMaintainence(Sites.Agency, ConfigurationManager.AppSettings["UnderMaintenanceUrl"]);

            if (!isUnderMaintenance)
            {
                var appLogs = _appLogService.GetAppLog(criteria).ToList();

                //System.Console.WriteLine("Get Log Data from Database:" + appLogs.Count().ToString());
                //foreach (var applog in appLogs)
                //{
                //    System.Console.WriteLine(applog.Message);
                //}

                var ignoreAppLogsXmlPath = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/IgnoreMessages.xml";

                if (appLogs.Count > 0)
                {
                    string groupChatXmlPath = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/GroupChat.xml";

                    appLogs = _appLogService.MappGroupChatAndGenerateUrlAndReferrer(groupChatXmlPath, appLogs).ToList();
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
            if (appLog.GroupChat != null && !IsIgnoreMessage(appLogsIgnored, appLog.Message))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Directory.GetCurrentDirectory() + @"/logs.txt", true))
                {
                    file.WriteLine(appLog.Category.CategoryName);
                    file.WriteLine(appLog.Message);
                }

                var vars = new Dictionary<string, string>
                                {
                                    { "category", appLog.Category.CategoryName },
                                    { "timestamp", appLog.Timestamp.ToShortTimeString() + " (GMT-4)" },
                                    { "severity", appLog.Severity },
                                    { "message", appLog.Message },
                                    { "url", appLog.LogUrl },
                                    { "referrer", appLog.Referrer },
                                    { "domain", appLog.AppDomainName },
                                    { "machine", appLog.Machine.MachineIP }
                                };
                var targets = appLog.GroupChat.Targets.Split(new char[','], StringSplitOptions.RemoveEmptyEntries);

                foreach (var target in targets)
                {
                    SendMessage(target, vars);
                }
            }
        }

        private static bool IsIgnoreMessage(IList<AppLog> appLogs, string message)
        {
            foreach (var applog in appLogs)
            {
                if (message.IndexOf(applog.Message, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
