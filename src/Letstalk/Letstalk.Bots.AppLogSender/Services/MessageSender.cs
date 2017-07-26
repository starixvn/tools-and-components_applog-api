using System;
using System.Linq;
using Letstalk.Bots.AppLogSender.Models;
using ONEbook.NotificationService;
using Fanex.Logging;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface MessageSender
    {
        void Run();
        MessageSender GetDataFrom(DateTime fromDate);
        MessageSender OnProduction(bool isProduction = true);
    }

    public class MessageSenderImpl : MessageSender
    {
        private readonly GroupManager groupManager;
        private readonly LogContentService logContentService;
        private readonly LetstalkSender sender;
        private DateTime fromDate;
        private bool isProduction;

        public MessageSenderImpl(GroupManager groupManager = null,
            LogContentService logContentService = null,
            LetstalkSender sender = null)
        {
            this.groupManager = groupManager ?? new GroupManagerImpl();
            this.logContentService = this.logContentService ?? new LogContentServiceImpl();
            this.sender = sender ?? new LetstalkSenderImpl();
        }

        public MessageSender GetDataFrom(DateTime fromDate)
        {
            this.fromDate = fromDate;
            return this;
        }

        public MessageSender OnProduction(bool isProduction = true)
        {
            this.isProduction = isProduction;
            return this;
        }

        public void Run()
        {
            var logContent = logContentService.GetLatestData(this.fromDate, this.isProduction);

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