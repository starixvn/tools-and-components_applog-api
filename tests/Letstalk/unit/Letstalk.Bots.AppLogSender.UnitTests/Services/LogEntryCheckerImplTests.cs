using System;
using System.Collections.Generic;
using System.Linq;
using Letstalk.Bots.AppLogSender.Models;
using Letstalk.Bots.AppLogSender.Services;
using Xunit;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    [Trait("Services", "ContentValidator")]
    public class LogEntryCheckerImplTests
    {
        private readonly IEnumerable<string> IgnoreTextList = new List<string> { "ignore", "hello" };
        private readonly IEnumerable<string> EmptyIgnoreList = Enumerable.Empty<string>();
        public LogEntry logEntry = new LogEntry { FormattedMessage = "Message: Content contains ignore text" };
              
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
            var validator = new LogEntryCheckerImpl(EmptyIgnoreList);

            var expected = validator.IsOK(logEntry);

            Assert.True(expected);
        }
            
        [Fact]
        public void IsOK_LogEntryIsNull_ThrowArgumentNullException()
        {
            var validator = new LogEntryCheckerImpl(EmptyIgnoreList);

            Assert.Throws<ArgumentNullException>(() => validator.IsOK(null));
        }
    }
}