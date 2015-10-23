namespace AppLogApi.Repositories.Impls
{
    using System.Collections.Generic;
    using System.Linq;
    using AppLogApi.Models.AppLog;
    using AppLogApi.Repositories.Interfaces;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AppLogRepository : IAppLogRepository
    {
        public IList<AppLogViewModel> GetAppLog(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IList<AppLogViewModel> appLogs = db.Query<AppLogViewModel, Category, Machine, AppLogViewModel>(
                     (appLog, category, machine) =>
                     {
                         appLog.Category = category;
                         appLog.Machine = machine;

                         return appLog;
                     },
                     "CategoryId, MachineId",
                     criteria.GetSpParams()).ToList();

                return appLogs;
            }
        }
    }
}
