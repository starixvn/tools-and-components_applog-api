namespace AppLogApi.Services.Impls
{
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
