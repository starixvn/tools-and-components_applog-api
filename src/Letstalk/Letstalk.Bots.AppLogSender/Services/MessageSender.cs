using System;
using System.Linq;
using Letstalk.Bots.AppLogSender.Models;
using ONEbook.NotificationService;
using Fanex.Logging;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface MessageSender
    {
        void Run(DateTime fromDate, bool isProduction);       
    }

    public class MessageSenderImpl : MessageSender
    {
        private readonly GroupManager groupManager;
        private readonly LogMessageService logContentService;
        private readonly LetstalkSender sender;
      
        public MessageSenderImpl(GroupManager groupManager = null,
            LogMessageService logContentService = null,
            LetstalkSender sender = null)
        {
            this.groupManager = groupManager ?? new GroupManagerImpl();
            this.logContentService = logContentService ?? new LogMessageServiceImpl();
            this.sender = sender ?? new LetstalkSenderImpl();
        }
       
        public void Run(DateTime fromDate, bool isProduction)
        {
            var logContent = logContentService.GetLatestData(fromDate, isProduction);

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