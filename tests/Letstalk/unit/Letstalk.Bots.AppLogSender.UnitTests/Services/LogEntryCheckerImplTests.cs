using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Letstalk.Bots.AppLogSender.Models;
using Letstalk.Bots.AppLogSender.Services;
using Xunit;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    [Trait("Services", "ContentValidator")]
    public class LogEntryCheckerImplTests
    {
        private readonly IEnumerable<string> IgnoreTextList = new List<string> { "ignore", "hello" };
        public LogEntry logEntry = new LogEntry { Message = "Message contains ignore text, hello world" };

        [Fact]
        public void IsOK_LogEntryIsOK_ReturnTrue()
        {
            var validator = new LogEntryCheckerImpl();

            var expected = validator.IsOK(logEntry);

            Assert.True(expected);
        }

        [Fact]
        public void IsOK_LogEntryContainsAnyIgnoredText_ReturnFalse()
        {
            var validator = new LogEntryCheckerImpl(IgnoreTextList);

            var expected = validator.IsOK(logEntry);

            Assert.False(expected);
        }

        [Fact]
        public void IsOK_IgnoredTextListIsNull_ReturnTrue()
        {
            var validator = new LogEntryCheckerImpl(null);

            var expected = validator.IsOK(logEntry);

            Assert.True(expected);
        }

        [Fact]
        public void IsOK_LogEntryIsNull_ThrowArgumentNullException()
        {
            var validator = new LogEntryCheckerImpl(null);

            Assert.Throws<ArgumentNullException>(() => validator.IsOK(null));
        }
    }
}