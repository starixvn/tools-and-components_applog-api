using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Letstalk.Bots.AppLogSender.Models;
using ONEbook.NotificationService;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface LetstalkSender
    {
        void Send(string group, IContent content);

        void SendAsync(string group, IContent content);
    }

    public class LetstalkSenderImpl : LetstalkSender
    {
        public LetstalkSenderImpl(string[] serverUrls = null)
        {
            var serverUris = serverUrls ??
                AppSettings.Service.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            Notification.Initialize(serverUris, AppSettings.AuthKey);
        }

        public void Send(string group, IContent content)
        {
            Notification.Send(group, content.Message);
        }

        public void SendAsync(string group, IContent content)
        {
            Notification.SendAsync(group, content.Message);
        }
    }
}