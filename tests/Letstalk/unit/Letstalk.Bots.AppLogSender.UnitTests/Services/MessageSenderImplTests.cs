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
            logEntryChecker.IsOK(Arg.Any<LogEntry>()).ReturnsForAnyArgs(false);
            repository
                .Fetch<LogEntry>(Arg.Any<ICriteria>())
                .ReturnsForAnyArgs(Enumerable.Empty<LogEntry>());
            var logContentService = new LogMessageServiceImpl(logEntryChecker, repository);

            var msgSender = new MessageSenderImpl(groupManager, logContentService, letstalkSender);

            msgSender.Run(DateTime.Now, true);

            // Act & Assert
            letstalkSender.DidNotReceiveWithAnyArgs().SendAsync(Arg.Any<string>(), Arg.Any<IMessage>());
        }

        [Fact]
        public void Run_ValidLogEntryHasRelationToAGroup_InvokeLetstalkSendAsync()
        {
            var logEntry = new LogEntry { LogId = 1, Message = "IContent 1", CategoryName = "Category 1" };
            var group = "my group";

            logEntryChecker.IsOK(Arg.Any<LogEntry>()).ReturnsForAnyArgs(true);
            groupManager.GetGroupByCategory(Arg.Any<string>()).ReturnsForAnyArgs(group);
            repository
                .Fetch<LogEntry>(Arg.Any<ICriteria>())
                .ReturnsForAnyArgs(new List<LogEntry> { logEntry });

            var logContentService = new LogMessageServiceImpl(logEntryChecker, repository);

            var msgSender = new MessageSenderImpl(groupManager, logContentService, letstalkSender);

            msgSender.Run(DateTime.Now, true);

            // Act & Assert
            letstalkSender.Received().SendAsync(group, new Message(logEntry)) ;
        }
    }
}