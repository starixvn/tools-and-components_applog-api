using Letstalk.Bots.AppLogSender.Models;
using ONEbook.NotificationService;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface LetstalkSender
    {
        void SendAsync(string group, IMessage content);
    }

    public class LetstalkSenderImpl : LetstalkSender
    {
        public LetstalkSenderImpl(string[] serviceUri = null)
        {
            Notification.Initialize(serviceUri ?? AppSettings.ServiceUri, AppSettings.AuthKey);
        }

        public void SendAsync(string group, IMessage content)
        {
            Notification.SendAsync(group, content.Content);
        }
    }
}