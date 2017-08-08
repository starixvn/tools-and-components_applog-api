using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using Letstalk.Bots.AppLogSender.Models;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface ILogMessageService
    {
        IEnumerable<IMessage> GetLatestData(DateTime fromDate, bool isProduction);
    }

    public class LogMessageServiceImpl : ILogMessageService
    {
        private readonly ILogEntryChecker checker;
        private readonly IDynamicRepository repository;

        public LogMessageServiceImpl(
            ILogEntryChecker checker = null,
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