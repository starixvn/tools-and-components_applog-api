using Letstalk.Bots.AppLogSender.Services;
using System;
using Xunit;
using Letstalk.Bots.AppLogSender.Models;
using NSubstitute;
using System.Linq;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    public class MessageSenderImplTests
    {
        public MessageSenderImplTests()
        {

        }

        [Fact]
        public void Run_NoLatestData_NoCallSendAsyncMessageWasInvoked()
        {
            //// Arrange
            //IContent content = null;
            //var logContentService = Substitute.For<LogContentService>();
            //logContentService
            //    .GetLatestData(Arg.Any<DateTime>(), Arg.Any<bool>())
            //    .ReturnsForAnyArgs(Enumerable.Empty<IContent>());

            //var sender = new MessageSenderImpl(logContentService)

            //// Act & Assert
            //Assert.Throws<ArgumentNullException>(() => sender.Send(string.Empty, content));
        }
    }
}