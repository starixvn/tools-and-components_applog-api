using AppLogApi.Services.Impls;
using AppLogApi.Services.Interfaces;
using Fanex.Data.Configuration;
using ONEbook.NotificationService;
using Sunplus.UMControllerLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Fanex.AutoBotChat.DBANotificationConsole
{
    internal static class Program
    {
        private static IDBANotificationLogService _DBANotificationLogService;
        private static UMBusiness _umService;
        private static string _environmentServers = string.Empty;
        private static string _environmentAuthKey = string.Empty;

        private static void Main(string[] args)
        {
            DbSettingProviderManager.Current.Start();
            _DBANotificationLogService = new DBANotificationLogService();
            _umService = new UMBusiness();
            _environmentServers = ConfigurationManager.AppSettings["Environtment-Servers"];
            _environmentAuthKey = ConfigurationManager.AppSettings["Environtment-AuthKey"];

            Process();
        }

        private static void Process()
        {
            bool isUnderMaintenance = _umService.isUnderMaintainence(Sites.Agency, ConfigurationManager.AppSettings["UnderMaintenanceUrl"]);
            if (!isUnderMaintenance)
            {
                var dbaNotificationLogs = _DBANotificationLogService.GetDBANotificationLog().ToList();
                if (dbaNotificationLogs.Count > 0)
                {
                    foreach (var dbaNotificationLog in dbaNotificationLogs)
                    {
                        var vars = new Dictionary<string, string>
                            {
                                { "Title", dbaNotificationLog.Title },
                                { "ServerName", dbaNotificationLog.ServerName },
                                { "LogDate", dbaNotificationLog.LogDate.ToLongDateString() },
                                { "Message", dbaNotificationLog.MsgInfo }
                            };
                        var target = dbaNotificationLog.TargetName;

                        SendMessage(target, vars);
                    }
                }
            }
        }

        private static void SendMessage(string targetName, Dictionary<string, string> vars)
        {
            var serverUrls = _environmentServers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Notification.Initialize(serverUrls, _environmentAuthKey);

            Notification.Send(targetName, vars);
        }
    }
}