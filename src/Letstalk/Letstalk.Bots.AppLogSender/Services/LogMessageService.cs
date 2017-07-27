using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Letstalk.Bots.AppLogSender.Models;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface LogMessageService
    {
        IEnumerable<IMessage> GetLatestData(DateTime fromDate, bool isProduction);
    }

    public class LogMessageServiceImpl : LogMessageService
    {
        private readonly LogEntryChecker checker;
        private readonly IDynamicRepository repository;

        public LogMessageServiceImpl(
            LogEntryChecker checker = null,
            IDynamicRepository repository = null)
        {
            this.checker = checker ?? new LogEntryCheckerImpl();
            this.repository = repository ?? new DynamicRepository();
        }

        public IEnumerable<IMessage> GetLatestData(DateTime fromDate, bool isProduction)
            => repository
            .Fetch<LogEntry>(new LogEntryCriteria(fromDate, isProduction))
            .Where(logEntry => checker.IsOK(logEntry))
            .Select(logEntry => new Message(logEntry));
    }

    public class LogEntryCriteria : CriteriaBase
    {
        public LogEntryCriteria(DateTime fromDate, bool isProduction = true)
        {
            FromDate = fromDate;
            IsProduction = isProduction;
        }

        public DateTime FromDate { get; } 

        public bool IsProduction { get; }

        public override string GetSettingKey() => "IS_SelAppLog_All";

        public override bool IsValid() => FromDate <= DateTime.Now;
    }
}