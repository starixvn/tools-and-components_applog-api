using System;
using Letstalk.Bots.AppLogSender.Models;
using ONEbook.NotificationService;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface MessageSender
    {
        MessageSender Send(IContent content);

        bool To(string group);
    }

    public class MessageSenderImpl : MessageSender
    {
        public IContent Content { get; set; }
        public MessageSender Send(IContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.Content = content;

            return this;
        }

        public bool To(string group)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                return false;
            }

            var serverUrls = AppSettingService.NotificationServer.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Notification.Initialize(serverUrls, AppSettingService.NotificationAuthKey);

            return Notification.Send(group, this.Content.Message);
        }
    }
}