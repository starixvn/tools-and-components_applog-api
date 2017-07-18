using System;
using System.Collections.Generic;
using System.Linq;
using Letstalk.Bots.AppLogSender.Models;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface LogEntryChecker
    {
        bool CannotSkipThisLog(LogEntry entry);
    }

    public class LogEntryCheckerImpl : LogEntryChecker
    {
        private readonly IEnumerable<string> ignoreTextList;

        public LogEntryCheckerImpl(IEnumerable<string> ignoreTextList = null)
        {
            this.ignoreTextList = ignoreTextList ?? GetIgnoreTextList();
        }

        public bool CannotSkipThisLog(LogEntry logEntry)
        {
            if (logEntry == null)
            {
                throw new ArgumentNullException(nameof(logEntry));
            }

            return logEntry.IsValid
                && (ignoreTextList == null
                || !ignoreTextList.Any(ingoreText => logEntry.Message.Contains(ingoreText)));
        }

        private IEnumerable<string> GetIgnoreTextList()
        {
            // TODO: Terry will load data from config file here
            return Enumerable.Empty<string>();
        }
    }
}