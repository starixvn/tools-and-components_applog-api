namespace AppLogApi.Services.Impls
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using AppLogApi.Criteria;
    using AppLogApi.Models.AppLog;
    using AppLogApi.Repositories.Impls;
    using AppLogApi.Repositories.Interfaces;
    using AppLogApi.Services.Interfaces;
    using Fanex.Common.Helper;

    public class AppLogService : IAppLogService
    {
        private readonly IAppLogRepository _repository;

        public AppLogService()
            : this(null)
        {
        }

        public AppLogService(IAppLogRepository repository)
        {
            _repository = repository ?? new AppLogRepository();
        }

        public IList<AppLogViewModel> GetAppLog(GetAppLogCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var appLogs = new List<AppLogViewModel>();

            if (criteria.IsValid())
            {
                appLogs = _repository.GetAppLog(criteria).ToList();
                appLogs = GenerateUrlAndReferrer(appLogs);
            }

            return appLogs;
        }

        public IList<AppLogViewModel> MappGroupChat(string groupChatXmlPath, IList<AppLogViewModel> appLogs)
        {
            IList<GroupChat> groupChats = GetGroupChat(groupChatXmlPath);

            if (groupChats != null && groupChats.Count > 0)
            {
                appLogs.Select(log =>
                {
                    var groupChat = groupChats
                        .FirstOrDefault(g => g.LogCategory.Equals(log.Category.CategoryName, System.StringComparison.InvariantCultureIgnoreCase));

                    if (groupChat != null)
                    {
                        log.GroupChat = groupChat;
                    }

                    return log;
                }).ToList();
            }

            return appLogs;
        }

        private List<AppLogViewModel> GenerateUrlAndReferrer(List<AppLogViewModel> appLogs)
        {
            appLogs.Select(log =>
            {
                string message = GetContentFromMessage(log.Message, string.Empty);
                string url = GetContentFromMessage(log.Message, "Url:");
                string referrer = GetContentFromMessage(log.Message, "REFERRER:");

                if (string.IsNullOrWhiteSpace(url))
                {
                    url = "N/A";
                }

                if (string.IsNullOrWhiteSpace(referrer))
                {
                    referrer = "N/A";
                }

                log.Message = message;
                log.Url = url;
                log.Referrer = referrer;

                return log;
            }).ToList();

            return appLogs;
        }

        private string GetContentFromMessage(string message, string contentKey)
        {
            string resultContent = string.Empty;
            int startIndex = 0;
            int endIndex = 0;
            int length = 0;

            int contentIndex = message.IndexOf(contentKey, System.StringComparison.InvariantCultureIgnoreCase);
            if (contentIndex != -1)
            {
                startIndex = contentIndex + contentKey.Length;
                endIndex = message.IndexOf(Environment.NewLine, startIndex, System.StringComparison.InvariantCultureIgnoreCase);

                if (endIndex != -1)
                {
                    length = endIndex - startIndex;
                    resultContent = message.Substring(startIndex, length);
                }
            }

            return resultContent;
        }

        private IList<GroupChat> GetGroupChat(string groupChatXmlPath)
        {
            var groupChats = new List<GroupChat>();

            if (File.Exists(groupChatXmlPath))
            {
                var deserializer = new XmlSerializer(typeof(List<GroupChat>), new XmlRootAttribute("GroupChats"));

                using (FileStream stream = File.OpenRead(groupChatXmlPath))
                {
                    groupChats = deserializer.Deserialize(stream) as List<GroupChat>;
                }
            }

            return groupChats;
        }
    }
}
