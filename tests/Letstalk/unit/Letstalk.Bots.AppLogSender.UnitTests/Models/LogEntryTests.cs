using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Letstalk.Bots.AppLogSender.Models;
using Xunit;
using System.Collections.Immutable;

namespace Letstalk.Bots.AppLogSender.UnitTests.Models
{
    [Trait("Model", "LogEntry")]
    public class LogEntryTests
    {
        private ImmutableList<string> IgnoreTextList = ImmutableList.Create("hello");

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void IsValid_NullOrWhiteSpaceMessage_ReturnFalse(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.False(logEntry.IsValid);
        }

        //[Theory]
        //[InlineData("hello world", "hello,text")]
        //[InlineData("hello elixir", "hello")]
        //public void IsValid_MessageContainsIgnoredText_ReturnFalse(string message, string ignoreText)
        //{
        //    IgnoreTextList = ImmutableList.Create(ignoreText.Split(','));
        //    var logEntry = new LogEntry()
        //    {
        //        IContent = message
        //    };

        //    Assert.False(logEntry.IsValid(IgnoreTextList));
        //}

        //[Fact]
        //public void IsValid_IgnoredTextListIsEmpty_ReturnTrue()
        //{
        //    IgnoreTextList = ImmutableList.Create<string>();

        //    var logEntry = new LogEntry()
        //    {
        //        IContent = "Any text"
        //    };

        //    Assert.True(logEntry.IsValid(IgnoreTextList));
        //}

        //[Fact]
        //public void IsValid_IgnoreTextListIsNull_ReturnTrue()
        //{
        //    IgnoreTextList = null;

        //    var logEntry = new LogEntry()
        //    {
        //        IContent = "Any text"
        //    };

        //    Assert.True(logEntry.IsValid(IgnoreTextList));
        //}
    }
}