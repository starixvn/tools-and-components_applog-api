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

        public bool IsOK(LogEntry logEntry)
        {
            if (logEntry == null)
            {
                throw new ArgumentNullException(nameof(logEntry));
            }

            return logEntry.IsValid
                && (ignoreTextList == null
                || !ignoreTextList.Any(ingoreText => logEntry.Message.Contains(ingoreText)));
        }

        private static IEnumerable<string> GetIgnoreTextList()
        {
            var configPath = AppDomain.CurrentDomain.BaseDirectory + "/Configurations/SkipTheseMessages.txt";
            if (File.Exists(configPath))
            {
                var ignoreMessages = File.ReadLines(configPath);

                return ignoreMessages;
            }

            return null;
        }
    }
}