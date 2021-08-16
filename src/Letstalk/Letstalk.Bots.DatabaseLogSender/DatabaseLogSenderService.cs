using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using Fanex.Logging;
using Letstalk.Bots.DatabaseLogSender.Models;
using Letstalk.Bots.DatabaseLogSender.Services;
using ONEbook.NotificationService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Letstalk.Bots.DatabaseLogSender
{
    public partial class DatabaseLogSenderService : ServiceBase
    {
        private readonly ILogmessageService logMessageService;
        private readonly UMService umService;

        public DatabaseLogSenderService()
        {
            InitializeComponent();
            logMessageService = new LogMessageService();
            umService = new UMServiceImpl(AppSetting.OneBookUMUrl);

            DbSettingProviderManager
               .StartNewSession()
               .Use(new OldConnectionStringProvider())
               .UseDefaultDbSettingProvider("~/App_Data")
               .Run();
        }

        protected override void OnStart(string[] args)
        {
            var timer = new Timer
            {
                Interval = AppSetting.Interval * 1000
            };
            timer.Elapsed += Ontimer;
            timer.Start();
        }

        protected override void OnStop()
        {
            // Method intentionally left empty.
        }

        private void Ontimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                bool isUnderMaintenance = umService.IsUM();

                if (!isUnderMaintenance)
                {
                    var logs = logMessageService.GetLatestData();

                    if (logs.Any())
                    {
                        PostMessage(logs);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }
        }

        private static void PostMessage(IList<LogEntry> logs)
        {
            foreach (var log in logs)
            {
                try
                {
                    var logDate = log.LogDate.ToString("yyyy-MM-dd hh:mm:ss tt");

                    // letstalk
                    var letstalkLogTemplate = new Dictionary<string, string>
                            {
                                { "Title", log.Title },
                                { "ServerName", log.ServerName },
                                { "LogDate", logDate },
                                { "Message", log.MsgInfo }
                            };
                    var target = log.TargetName;

                    MessageSenderService.LetstalkSender(target, letstalkLogTemplate);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message, ex);
                }
            }
        }
    }
}