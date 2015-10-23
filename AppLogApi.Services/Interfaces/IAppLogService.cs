namespace AppLogApi.Services.Interfaces
{
    using System.Collections.Generic;
    using AppLogApi.Criteria;
    using AppLogApi.Models.AppLog;

    public interface IAppLogService
    {
        IList<AppLogViewModel> GetAppLog(GetAppLogCriteria criteria);

        IList<AppLogViewModel> MappGroupChat(string groupChatXmlPath, IList<AppLogViewModel> appLogs);
    }
}
