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
    [Trait("Service", "LogMessageService")]
    public class LogMessageServiceImplTests
    {
        private IList<LogEntry> latestData;        

        public LogMessageServiceImplTests()
        {
            latestData = new List<LogEntry>
            {
                new LogEntry { LogId = 1, Message = "Content 1" },
                new LogEntry { LogId = 2, Message = "Contain ignore message" },
                new LogEntry { LogId = 3, Message = "" }
            };
        }

        [Fact]
        public void GetLatestData_NeverReturnContentCreatedByInvalidLogEntry()
        {
            var validLogEntry = new Message(latestData[0]);
            var logEntryChecker = new LogEntryCheckerImpl(new List<string> { "ignore message" });            
            var repository = Substitute.For<IDynamicRepository>();
            repository
                .Fetch<LogEntry>(new LogEntryCriteria(DateTime.Now, true))
                .ReturnsForAnyArgs(latestData);            

            var logContentService = new LogMessageServiceImpl(logEntryChecker, repository);

            var contentCollection = logContentService.GetLatestData(DateTime.Now, true);

            Assert.True(contentCollection.Any(content => validLogEntry.LogId == content.LogId));
        }      

    }
}