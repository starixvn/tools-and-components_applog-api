using System.ServiceProcess;
using System.Timers;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using Letstalk.Bots.DatabaseLogSender.Services;
using System.Linq;
using System.Collections.Generic;
using System;
using ONEbook.NotificationService;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Letstalk.Bots.DatabaseLogSender.Models;
using Fanex.Logging;
using System.Text;
using Sunplus.UMControllerLibrary;
using System.Globalization;

namespace Letstalk.Bots.DatabaseLogSender
{
    public partial class DatabaseLogSenderService : ServiceBase
    {
        private readonly ILogmessageService logMessageService;
        private readonly UMBusiness _umService;
        public DatabaseLogSenderService()
        {
            InitializeComponent();
            logMessageService = new LogMessageService();
            _umService = new UMBusiness();

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
                bool isUnderMaintenance = _umService.isUnderMaintainence(Sites.Agency, AppSetting.UnderMaintenanceService);

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
                LogService.Error(ex.Message);
            }
        }

        private static void PostMessage(IList<LogEntry> logs)
        {
            foreach (var log in logs)
            {

                var logDate = log.LogDate.ToString("yyyy-MM-dd hh:mm:ss tt");
                var breakLine = "<br/>";

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

                // Skype
                var skypeLogTemplate = new StringBuilder();
                skypeLogTemplate.Append("Title: " + log.Title);
                skypeLogTemplate.Append(breakLine);
                skypeLogTemplate.Append("Server Name: " + log.ServerName);
                skypeLogTemplate.Append(breakLine);
                skypeLogTemplate.Append("Log Date: " + logDate);
                skypeLogTemplate.Append(breakLine);
                skypeLogTemplate.Append("Message: " + log.MsgInfo);

                MessageSenderService.SkypeSender(new MessageModel
                {
                    ConversationId = log.SkypeGroupId,
                    Message = skypeLogTemplate.ToString()
                });
            }
        }
    }
}