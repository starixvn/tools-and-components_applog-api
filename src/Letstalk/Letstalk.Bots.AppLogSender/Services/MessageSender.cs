using System;
using System.Linq;
using Letstalk.Bots.AppLogSender.Models;
using ONEbook.NotificationService;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface MessageSender
    {
        void Run();
    }

    public class MessageSenderImpl : MessageSender
    {
        private readonly GroupManager groupManager;
        private readonly LogContentService logContenGetter;
        private readonly LetstalkSender sender;

        public MessageSenderImpl(GroupManager groupManager = null,
            LogContentService logContentGetter = null,
            LetstalkSender sender = null)
        {
            this.groupManager = groupManager ?? new GroupManagerImpl(null);
            this.logContenGetter = logContenGetter ?? new LogContentServiceImpl();
            this.sender = sender ?? new LetstalkSenderImpl();
        }

        public void Run()
        {
            var logContent = logContenGetter.GetLatestData();

            if (logContent != null)
            {
                foreach (var content in logContent)
                {
                    var group = groupManager.GetGroupByCategory(content.Category);

                    if (!string.IsNullOrWhiteSpace(group))
                    {
                        sender.SendAsync(group, content);
                    }
                }
            }
        }
    }
}