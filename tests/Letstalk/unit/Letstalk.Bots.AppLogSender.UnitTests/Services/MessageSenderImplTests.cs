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
    [Trait("Service", "MessageSende")]
    public class MessageSenderImplTests
    {
        LogEntryChecker logEntryChecker;
        IDynamicRepository repository;
        LetstalkSender letstalkSender;
        GroupManager groupManager;

        public MessageSenderImplTests()
        {
            logEntryChecker = Substitute.For<LogEntryChecker>();
            repository = Substitute.For<IDynamicRepository>();
            letstalkSender = Substitute.For<LetstalkSender>();
            groupManager = Substitute.For<GroupManager>();            
        }

        [Fact]
        public void Run_NullOrEmptyLogData_DoNotInvokeGetGroupByCategory()
        {
            logEntryChecker.IsOK(new LogEntry()).ReturnsForAnyArgs(false);
            repository
                .Fetch<LogEntry>(new LogEntryCriteria(DateTime.Now, true))
                .ReturnsForAnyArgs(Enumerable.Empty<LogEntry>());
            var logContentService = new LogMessageServiceImpl(logEntryChecker, repository);

            var msgSender = new MessageSenderImpl(groupManager, logContentService, letstalkSender);

            msgSender.Run(DateTime.Now, true);

            // Act & Assert
            letstalkSender.DidNotReceiveWithAnyArgs().SendAsync(string.Empty, new Message(new LogEntry()));
        }

        [Fact]
        public void Run_ValidLogEntryHasRelationToAGroup_InvokeLetstalkSendAsync()
        {
            var logEntry = new LogEntry { LogId = 1, Message = "IContent 1", CategoryName = "Category 1" };
            var group = "my group";

            logEntryChecker.IsOK(new LogEntry()).ReturnsForAnyArgs(true);
            groupManager.GetGroupByCategory("category 1").ReturnsForAnyArgs(group);
            repository
                .Fetch<LogEntry>(new LogEntryCriteria(DateTime.Now, true))
                .ReturnsForAnyArgs(new List<LogEntry> { logEntry });

            var logContentService = new LogMessageServiceImpl(logEntryChecker, repository);

            var msgSender = new MessageSenderImpl(groupManager, logContentService, letstalkSender);

            msgSender.Run(DateTime.Now, true);

            // Act & Assert
            letstalkSender.Received().SendAsync(group, new Message(logEntry)) ;
        }
    }
}