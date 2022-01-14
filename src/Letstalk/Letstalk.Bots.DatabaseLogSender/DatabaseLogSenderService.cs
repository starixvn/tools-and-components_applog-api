using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Letstalk.Bots.DatabaseLogSender.Models;
using Letstalk.Bots.DatabaseLogSender.Services;
using Newtonsoft.Json;
using ONEbook.NotificationService;
using SharpRaven;
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

            LogManager
                 .SetDefaultLogCategory(AppSetting.SentryEnvironment)
                 .Use(new List<ILogEngine>
                 {
                     new SentryLogging(new SentryEngineOptions {
                        Dsn = new Dsn(AppSetting.SentryDsn),
                        Environment = AppSetting.SentryEnvironment
                     })
                 });

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

                    LogService.Info(JsonConvert.SerializeObject(logs));

                    if (logs.Any())
                    {
                        PostMessage(logs);
                    }
                }
                else
                {
                    LogService.Info("UM is starting. Pause sending messages!");
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }
        }

        private static void PostMessage(IList<Models.LogEntry> logs)
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