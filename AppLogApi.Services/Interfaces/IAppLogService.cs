namespace AppLogApi.Services.Interfaces
{
    using System.Collections.Generic;
    using AppLogApi.Criteria;
    using AppLogApi.Models.AppLog;

    public interface IAppLogService
    {
        IList<AppLogViewModel> GetAppLog(GetAppLogCriteria criteria);

        IList<AppLogViewModel> MappGroupChatAndGenerateUrlAndReferrer(string groupChatXmlPath, IList<AppLogViewModel> appLogs);

        IList<AppLog> GetIgnoreMessage(string xmlPath);
    }
}
