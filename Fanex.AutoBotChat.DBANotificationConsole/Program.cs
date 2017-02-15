using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogApi.Criteria;
using AppLogApi.Models.AppLog;
using AppLogApi.Services.Impls;
using AppLogApi.Services.Interfaces;
using Fanex.Data;
using Fanex.Data.Configuration;
using ONEbook.NotificationService;
using Sunplus.UMControllerLibrary;

namespace Fanex.AutoBotChat.DBANotificationConsole
{
    class Program
    {
        private static IDBANotificationLogService _DBANotificationLogService;
        private static UMBusiness _umService;
        private static int _interval = 0;
        private const int KeySize = 1024;
        private static string _environmentServers = string.Empty;
        private static string _environmentAuthKey = string.Empty;
        private static string _targetNames = string.Empty;
        static void Main(string[] args)
        {
            try
            {
                DbSettingProviderManager.Current.Start();
                _DBANotificationLogService = new DBANotificationLogService();
                _umService = new UMBusiness();
                _interval = Convert.ToInt32(ConfigurationManager.AppSettings["interval"]);
                _environmentServers = ConfigurationManager.AppSettings["Environtment-Servers"];
                _environmentAuthKey = ConfigurationManager.AppSettings["Environtment-AuthKey"];            
                                              
               Process();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.ReadKey();
            }
        }

        private static void Process()
        {
            var dbaNotificationLogs = new List<DBANotificationLogViewModel>();
            bool isUnderMaintenance = _umService.isUnderMaintainence(Sites.Agency, ConfigurationManager.AppSettings["UnderMaintenanceUrl"]);            
            if (!isUnderMaintenance)
            {
                dbaNotificationLogs = _DBANotificationLogService.GetDBANotificationLog().ToList();
                System.Console.WriteLine("appLogs counter item " + dbaNotificationLogs.Count());
                if (dbaNotificationLogs.Count > 0)
                {
                    foreach (var dbaNotificationLog in dbaNotificationLogs)
                    {
                        var vars = new Dictionary<string, string>()
                            {
                                { "Title", dbaNotificationLog.Title },
                                { "ServerName", dbaNotificationLog.ServerName },
                                { "LogDate", dbaNotificationLog.LogDate.ToString() },
                                { "Message", dbaNotificationLog.MsgInfo }
                            };
                        var target = dbaNotificationLog.TargetName; //_targetNames.Split(new char[','], StringSplitOptions.RemoveEmptyEntries);


                        System.Console.WriteLine(target);

                        SendMessage(target, vars);

                        //foreach (var target in targets)
                        //{
                        //    SendMessage(target, vars);
                        //}
                    }
                }
            }
        }

        private static bool SendMessage(string targetName, Dictionary<string, string> vars)
        {

            var serverUrls = _environmentServers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Notification.Initialize(serverUrls, _environmentAuthKey);

            return Notification.Send(targetName, vars, null, null);
        }
    }
}
