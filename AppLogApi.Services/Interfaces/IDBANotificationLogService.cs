using AppLogApi.Models.AppLog;
using System.Collections.Generic;

namespace AppLogApi.Services.Interfaces
{
    public interface IDBANotificationLogService
    {
        IList<DBANotificationLogViewModel> GetDBANotificationLog();
    }
}