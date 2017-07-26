using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using Letstalk.Bots.AppLogSender.Models;
using Letstalk.Bots.AppLogSender.Services;
using NSubstitute;
using Xunit;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    [Trait("Service", "LogContentService")]
    public class LogContentServiceImplTests
    {
        private IList<LogEntry> latestData;

        public LogContentServiceImplTests()
        {
            latestData = new List<LogEntry>();
            latestData.Add(new LogEntry { LogId = 1, Message = "Message 1" });
            latestData.Add(new LogEntry { LogId = 2, Message = "Contain ignore message" });
            latestData.Add(new LogEntry { LogId = 3, Message = "" });
        }

        [Fact]
        public void GetLatestData_NeverReturnContentCreatedByInvalidLogEntry()
        {
            var validLogEntry = new Content(latestData[0]);
            var logEntryChecker = new LogEntryCheckerImpl(new List<string> { "ignore message" });
            var repository = Substitute.For<IDynamicRepository>();
            repository
                .Fetch<LogEntry>(Arg.Any<LogEntryCriteria>())
                .Returns(latestData);            

            var logContentService = new LogContentServiceImpl(logEntryChecker, repository);

            var contentCollection = logContentService.GetLatestData(Arg.Any<DateTime>(), true);

            Assert.True(contentCollection.Any(content => validLogEntry.LogId == content.LogId));
        }
    }
}