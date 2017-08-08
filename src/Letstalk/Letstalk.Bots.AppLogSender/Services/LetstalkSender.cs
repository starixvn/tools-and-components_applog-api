using Letstalk.Bots.AppLogSender.Models;
using ONEbook.NotificationService;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface ILetstalkSender
    {
        void SendAsync(string group, IMessage content);
    }

    public class LetstalkSenderImpl : ILetstalkSender
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