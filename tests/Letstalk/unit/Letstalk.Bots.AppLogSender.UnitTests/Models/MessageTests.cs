using System;
using Letstalk.Bots.AppLogSender.Models;
using Xunit;


namespace Letstalk.Bots.AppLogSender.UnitTests.Models
{
    [Trait("Model", "Message")]
    public class MessageTests
    {
        [Fact]
        public void Message_LogEntryIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Message(null));
        }

        [Fact]
        public void Content_ReturnContentDictionary_ReturnTrue()
        {
            var logEntry = new LogEntry()
            {
                Timestamp = DateTime.Now,
                AppDomainName = "my domain name",
                CategoryId = 1,
                CategoryName = "my category",               
                MachineIP = "my machine ip",
                LogId = 1,
                Message = "Message: Additional non-parsable characters are at the end of the string. ",
                FormattedMessage = "Message: Additional non-parsable characters are at the end of the string. "
            };

            var message = new Message(logEntry);

            Assert.NotEmpty(message.Content);            
            Assert.Equal($"{logEntry.Timestamp.ToShortTimeString()} (GMT-4)", message.Content["timestamp"]);
            Assert.Equal(logEntry.CategoryName, message.Content["category"]);
            Assert.Equal(logEntry.TheMessage, message.Content["message"]);            
        }

        [Fact]
        public void GetHashCode_ReturnLogEntry_ReturnTrue()
        {
            var logEntry = new LogEntry()
            {
                Timestamp = DateTime.Now,
                AppDomainName = "my domain name",
                CategoryId = 1,
                CategoryName = "my category",
                MachineIP = "my machine ip",
                LogId = 1,
                Message = "Message: Additional non-parsable characters are at the end of the string. ",
                FormattedMessage = "Message: Additional non-parsable characters are at the end of the string. "
            };

            var message = new Message(logEntry);

            Assert.Equal(message.GetHashCode(), logEntry.GetHashCode());
        }
    }
}
