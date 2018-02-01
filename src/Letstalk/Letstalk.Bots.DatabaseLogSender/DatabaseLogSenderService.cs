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
                        foreach (var log in logs)
                        {
                            // letstalk
                            var vars = new Dictionary<string, string>
                            {
                                { "Title", log.Title },
                                { "ServerName", log.ServerName },
                                { "LogDate", log.LogDate.ToString("yyyy-MM-dd hh:mm:ss tt") },
                                { "Message", log.MsgInfo }
                            };
                            var target = log.TargetName;

                            SendMessage(target, vars);

                            // Skype
                            var logTemplate = new StringBuilder();
                            logTemplate.Append("Title: " + log.Title);
                            logTemplate.Append("<br/>");
                            logTemplate.Append("ServerName: " + log.ServerName);
                            logTemplate.Append("<br/>");
                            logTemplate.Append("LogDate: " + log.LogDate.ToString("yyyy-MM-dd hh:mm:ss tt"));
                            logTemplate.Append("<br/>");
                            logTemplate.Append("Message: " + log.MsgInfo);

                            SendMessage(new MessageModel
                            {
                                ConversationId = 1,
                                Message = logTemplate.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }
        }
        private static async Task SendMessage(string targetName, Dictionary<string, string> vars)
        {
            var serverUrls = AppSetting.LetstalkAPI.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Notification.Initialize(serverUrls, AppSetting.LetstalkKey);

            Notification.SendAsync(targetName, vars);
        }

        private static async Task SendMessage(MessageModel messageModel)
        {
            string apiBasePath = AppSetting.CoronaService;

            var customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync(apiBasePath + "api/SendMessage", messageModel);

            if (!response.IsSuccessStatusCode)
            {
                LogService.Error(string.Format("Failed to call the API. HTTP Status: {0}, Reason {1}, Response header {2}", response.StatusCode, response.ReasonPhrase, response.Headers.ToString()));
            }
        }
    }
}