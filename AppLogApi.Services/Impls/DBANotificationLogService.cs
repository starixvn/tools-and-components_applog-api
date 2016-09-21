using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLogApi.Criteria;
using AppLogApi.Models.AppLog;
using AppLogApi.Repositories.Impls;
using AppLogApi.Repositories.Interfaces;
using AppLogApi.Services.Interfaces;

namespace AppLogApi.Services.Impls
{
    public class DBANotificationLogService : IDBANotificationLogService
    {
        private readonly IDBANotificationLogRepository _repository;

        public DBANotificationLogService() : this(null) { }

        public DBANotificationLogService(IDBANotificationLogRepository repository)
        {
            _repository = repository ?? new DBANotificationLogRepository();
        }
        public IList<DBANotificationLogViewModel> GetDBANotificationLog()
        {
            var criteria = new GetDBANotificationLogCriteria();

            return _repository.GetDBANotificationLog(criteria);
        }
    }
}
