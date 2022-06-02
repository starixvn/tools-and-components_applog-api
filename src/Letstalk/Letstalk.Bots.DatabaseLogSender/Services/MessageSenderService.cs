using Fanex.Logging;
using Letstalk.Bots.DatabaseLogSender.Models;
using ONEbook.NotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Letstalk.Bots.DatabaseLogSender.Services
{
    public static class MessageSenderService
    {
        public static bool SendLetstalk(string targetName, Dictionary<string, string> vars)
        {
            var serverUrls = AppSetting.LetstalkAPI.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Notification.Initialize(serverUrls, AppSetting.LetstalkKey);

            return Notification.Send(targetName, vars);
        }
    }
}
