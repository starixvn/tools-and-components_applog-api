using Letstalk.Bots.AppLogSender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using System.Collections.Immutable;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface LogDataService
    {
        IEnumerable<LogEntry> GetLatestData();
    }

    public class LogDataServiceImpl : LogDataService
    {
        private readonly ContentValidator validator;
        private readonly IDynamicRepository repository;

        public LogDataServiceImpl(ContentValidator validator = null, DynamicRepository repository = null)
        {
            this.validator = validator ?? new ContentValidatorImpl();
            this.repository = repository ?? new DynamicRepository();
        }

        public IEnumerable<LogEntry> GetLatestData()
        {
            var logData = repository.Fetch<LogEntry>(new { FromDate = DateTime.Now });

            return logData.Where(logEntry => validator.IsValid(logEntry));
        }
    }
}