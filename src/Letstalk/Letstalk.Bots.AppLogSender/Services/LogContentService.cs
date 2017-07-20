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
    public interface LogContentService
    {
        IEnumerable<IContent> GetLatestData();
    }

    public class LogContentServiceImpl : LogContentService
    {
        private readonly LogEntryChecker checker;
        private readonly IDynamicRepository repository;

        public LogContentServiceImpl(
            LogEntryChecker checker = null,
            DynamicRepository repository = null)
        {
            this.checker = checker ?? new LogEntryCheckerImpl();
            this.repository = repository ?? new DynamicRepository();
        }

        public IEnumerable<IContent> GetLatestData()
            => repository
            .Fetch<LogEntry>(new LogEntryCriteria())
            .Where(logEntry => checker.IsOK(logEntry))
            .Select(logEntry => new Content(logEntry));
    }

    public class LogEntryCriteria : CriteriaBase
    {
        public LogEntryCriteria() : this(null, null)
        {
        }

        public LogEntryCriteria(DateTime? fromDate, bool? isProduction = true)
        {
            FromDate = fromDate.HasValue
                ? fromDate.Value : DateTime.Now.AddSeconds(-AppSettings.Interval);
            IsProduction = isProduction.HasValue
                ? isProduction.Value : AppSettings.IsProduction;
        }

        public DateTime FromDate { get; }

        public bool IsProduction { get; }

        public override string GetSettingKey() => "IS_SelAppLog_All_V2";

        public override bool IsValid() => true;
    }
}