namespace AppLogApi.Repositories.Impls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AppLogApi.Models.AppLog;
    using AppLogApi.Repositories.Interfaces;
    using Fanex.Data;
    using Fanex.Data.Repository;

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
