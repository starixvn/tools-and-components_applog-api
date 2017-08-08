using System;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface IMessageSender
    {
        void Run(DateTime fromDate, bool isProduction);
    }

    public class MessageSenderImpl : IMessageSender
    {
        private readonly IGroupManager groupManager;
        private readonly ILogMessageService logContentService;
        private readonly ILetstalkSender sender;

        public MessageSenderImpl(IGroupManager groupManager = null,
            ILogMessageService logContentService = null,
            ILetstalkSender sender = null)
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