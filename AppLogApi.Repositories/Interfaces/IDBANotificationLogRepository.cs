

namespace AppLogApi.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Models.AppLog;

    public interface IDBANotificationLogRepository
    {
        IList<DBANotificationLogViewModel> GetDBANotificationLog(ICriteria criteria);
    }
}
