namespace AppLogApi.Repositories.Interfaces
{
    using AppLogApi.Models.AppLog;
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface IAppLogRepository
    {
        IList<AppLogViewModel> GetAppLog(ICriteria criteria);
    }
}