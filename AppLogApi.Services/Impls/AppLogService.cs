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
        private readonly List<string> ignoreMessages = new List<string>();
        public AppLogService()
            : this(null)
        {
            ignoreMessages.Add("Thread was being aborted.");
            ignoreMessages.Add("Invalid attempt to read when no data is present.");
            ignoreMessages.Add("Object reference not set to an instance of an object.");
            ignoreMessages.Add("Server cannot set status after HTTP headers have been sent.");

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
            }

            return appLogs;
        }

        public IList<AppLogViewModel> MappGroupChatAndGenerateUrlAndReferrer(string groupChatXmlPath, IList<AppLogViewModel> appLogs)
        {
            var groupChats = GetGroupChat(groupChatXmlPath);
            var appLogList = new List<AppLogViewModel>();

            if (groupChats != null && groupChats.Count > 0)
            {
                appLogList = appLogs.Select(log =>
                 {
                     var groupChat = groupChats
                         .FirstOrDefault(g => g.LogCategory.Equals(log.Category.CategoryName, System.StringComparison.InvariantCultureIgnoreCase));

                     if (groupChat != null)
                     {
                         log.GroupChat = groupChat;
                     }

                    var generatedLog = GenerateUrlAndReferrer(log);

                    return generatedLog;

                 })
                .Where(x => x != null)
                .ToList();
            }

            return appLogList;
        }


        public IList<AppLog> GetIgnoreMessage(string xmlPath)
        {
            var appLogs = new List<AppLog>();

            if (File.Exists(xmlPath))
            {
                var deserializer = new XmlSerializer(typeof(List<AppLog>), new XmlRootAttribute("AppLogs"));

                using (FileStream stream = File.OpenRead(xmlPath))
                {
                    appLogs = deserializer.Deserialize(stream) as List<AppLog>;
                }
            }

            return appLogs;
        }

        private AppLogViewModel GenerateUrlAndReferrer(AppLogViewModel log)
        {
            string content = GetContentFromMessage(log.Message, string.Empty);
            string message = GetContentFromMessage(log.Message, "Message:");
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

            if (IgnoreMessage(message) == null)
            {
                return null;
            }

            log.Message = content;
            log.LogUrl = url;
            log.Referrer = referrer;

            return log;
        }

        private string IgnoreMessage(string message)
        {
            if (ignoreMessages.Contains(message))
            {
                return null;
            }

            return message;
        }

        private static string GetContentFromMessage(string message, string contentKey)
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

                if (endIndex == -1 && startIndex == contentIndex)
                {
                    endIndex = message.IndexOf(".", startIndex, System.StringComparison.InvariantCultureIgnoreCase);

                    if (endIndex == -1)
                    {
                        resultContent = message;
                    }
                }

                if (endIndex != -1)
                {
                    length = endIndex - startIndex;
                    resultContent = message.Substring(startIndex, length);
                }
            }

            return resultContent;
        }

        private static IList<GroupChat> GetGroupChat(string groupChatXmlPath)
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
