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
    public class ContentValidatorImplTests
    {
        private readonly IEnumerable<string> IgnoreTextList = new List<string> { "ignore", "hello" };
        public LogEntry logEntry = new LogEntry { Message = "Message contains ignore text, hello world" };

        [Fact]
        public void IsValid_LogEntryIsValid_ReturnTrue()
        {
            var validator = new ContentValidatorImpl();

            var expected = validator.IsValid(logEntry);

            Assert.True(expected);
        }

        [Fact]
        public void IsValid_LogEntryContainsAnyIgnoredText_ReturnFalse()
        {
            var validator = new ContentValidatorImpl(IgnoreTextList);

            var expected = validator.IsValid(logEntry);

            Assert.False(expected);
        }

        [Fact]
        public void IsValid_IgnoredTextListIsNull_ReturnTrue()
        {
            var validator = new ContentValidatorImpl(null);

            var expected = validator.IsValid(logEntry);

            Assert.True(expected);
        }
    }
}