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
    public interface LogContentGetter
    {
        IEnumerable<IContent> GetLatestData();
    }

    public class LogContentGetterImpl : LogContentGetter
    {
        private readonly LogEntryChecker checker;
        private readonly IDynamicRepository repository;

        public LogContentGetterImpl(
            LogEntryChecker checker = null,
            DynamicRepository repository = null)
        {
            this.checker = checker ?? new LogEntryCheckerImpl();
            this.repository = repository ?? new DynamicRepository();
        }

        public IEnumerable<IContent> GetLatestData()
            => repository
            .Fetch<LogEntry>(new { FromDate = DateTime.Now })
            .Where(logEntry => checker.IsValid(logEntry))
            .Select(logEntry => new Content(logEntry));
    }
}