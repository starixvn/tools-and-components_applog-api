namespace AppLogApi.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Models.AppLog;
    using System.Collections.Generic;

    public interface IDBANotificationLogRepository
    {
        IList<DBANotificationLogViewModel> GetDBANotificationLog(ICriteria criteria);
    }
}