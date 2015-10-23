namespace AppLogApi.Repositories.Interfaces
{
    using System.Collections.Generic;
    using AppLogApi.Models.AppLog;
    using Fanex.Data.Repository;

    public interface IAppLogRepository
    {
        IList<AppLogViewModel> GetAppLog(ICriteria criteria);
    }
}
