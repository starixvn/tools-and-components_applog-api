using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Letstalk.Bots.AppLogSender.Models;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface LogEntryChecker
    {
        bool IsOK(LogEntry entry);
    }

    public class LogEntryCheckerImpl : LogEntryChecker
    {
        private readonly IEnumerable<string> ignoreTextList;

        public LogEntryCheckerImpl(IEnumerable<string> ignoreTextList = null)
        {
            this.ignoreTextList = ignoreTextList ?? GetIgnoreTextList();
        }

        public bool IsOK(LogEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            return entry.IsValid
                && (ignoreTextList == null
                || !ignoreTextList.Any(ingoreText => entry.Message.Contains(ingoreText)));
        }

        private static IEnumerable<string> GetIgnoreTextList()
            => File.ReadAllLines(AppSettings.IgnoreMessagesFilePath);
    }
}