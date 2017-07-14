namespace AppLogApi.Services.Interfaces
{
    using AppLogApi.Criteria;
    using AppLogApi.Models.AppLog;
    using System.Collections.Generic;

    public interface IAppLogService
    {
        IList<AppLogViewModel> GetAppLog(GetAppLogCriteria criteria);

        IList<AppLogViewModel> MappGroupChatAndGenerateUrlAndReferrer(string groupChatXmlPath, IList<AppLogViewModel> appLogs);

        IList<AppLog> GetIgnoreMessage(string xmlPath);
    }
}