using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLogApi.Models.AppLog;

namespace AppLogApi.Services.Interfaces
{
    public interface IDBANotificationLogService
    {
        IList<DBANotificationLogViewModel> GetDBANotificationLog();
    }
}
