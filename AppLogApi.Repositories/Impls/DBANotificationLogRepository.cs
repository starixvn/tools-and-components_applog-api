namespace AppLogApi.Repositories.Impls
{
    using AppLogApi.Models.AppLog;
    using AppLogApi.Repositories.Interfaces;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using System.Linq;

    public class DBANotificationLogRepository : IDBANotificationLogRepository
    {
        public IList<DBANotificationLogViewModel> GetDBANotificationLog(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IList<DBANotificationLogViewModel> dbaNotificationLogs = db.Query<DBANotificationLogViewModel>().ToList();

                return dbaNotificationLogs;
            }
        }
    }
}