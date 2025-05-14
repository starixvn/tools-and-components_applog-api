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
        private readonly MattermostService mattermostService;

        public DatabaseLogSenderService()
        {
            InitializeComponent();
            logMessageService = new LogMessageService();
            umService = new UMServiceImpl(AppSetting.OneBookUMUrl);
            mattermostService = new MattermostService(AppSetting.MattermostUrl, AppSetting.MattermostBotToken);

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

        private async void Ontimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                bool isUnderMaintenance = umService.IsUM();

                if (!isUnderMaintenance)
                {
                    var logs = logMessageService.GetLatestData();

                    if (logs.Any())
                    {
                        LogService.Info(JsonConvert.SerializeObject(logs));

                        await PostMessage(logs);
                    }
                    else if (AppSetting.DebugSentryLog)
                    {
                        LogService.Info(JsonConvert.SerializeObject(logs));
                    }
                    else
                    {
                        //Do nothing
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

        private async Task PostMessage(IList<Models.LogEntry> logs)
        {
            foreach (var log in logs)
            {
                try
                {
                    var logDate = log.LogDate.ToString("yyyy-MM-dd hh:mm:ss tt");
                    var formattedMessage = FormatLogMessage(log, logDate);
                    var channelId = log.TargetName; // Using the target name as channel ID

                    bool isSuccessful = await mattermostService.SendMessageAsync(channelId, formattedMessage);

                    if (!isSuccessful)
                    {
                        LogService.Error($"There is an error when sending a message ({log.MsgInfo}) to Mattermost (Channel ID: {channelId})");
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message, ex);
                }
            }
        }

        private string FormatLogMessage(Models.LogEntry log, string logDate)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"### {log.Title}");
            messageBuilder.AppendLine($"**Server:** {log.ServerName}");
            messageBuilder.AppendLine($"**Time:** {logDate}");
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("**Message:**");
            messageBuilder.AppendLine($"```");
            messageBuilder.AppendLine(log.MsgInfo);
            messageBuilder.AppendLine("```");

            return messageBuilder.ToString();
        }
    }
}