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

        [Theory]
        [InlineData("Message: The stored procedure 'NAP_Admin_VerifySiteName' doesn't exist.\r\nSOURCE: System.Data.\r\nDOMAIN: localhost.\r\nREFERRER: hello world\r\n")]
        public void Referrer_GetRefererFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal(" hello world", logEntry.Referrer);

        }

        [Theory]
        [InlineData("Hello World")]
        public void Referrer_GetNAFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal("N/A", logEntry.Referrer);

        }

        [Theory]
        [InlineData("Message: The stored procedure 'NAP_Admin_VerifySiteName' doesn't exist.\r\nSOURCE: System.Data.\r\nDOMAIN: localhost.\r\nClient Ip: hello world\r\n")]
        public void ClientIp_GetClientIpFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal(" hello world", logEntry.ClientIp);

        }

        [Theory]
        [InlineData("Hello World")]
        public void ClientIp_GetNAFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal("N/A", logEntry.ClientIp);

        }

        [Theory]
        [InlineData("Message: The stored procedure 'NAP_Admin_VerifySiteName' doesn't exist.\r\nSOURCE: System.Data.\r\nDOMAIN: localhost.\r\nMachine: hello world\r\n")]
        public void Machine_GetMachineFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal(" hello world", logEntry.Machine);

        }

        [Theory]
        [InlineData("Hello World")]
        public void Machine_GetNAFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal("N/A", logEntry.Machine);

        }

        [Theory]
        [InlineData("Message: The stored procedure 'NAP_Admin_VerifySiteName' doesn't exist.\r\nSOURCE: System.Data.\r\nDOMAIN: localhost.\r\nCustId: hello world\r\n")]
        public void CustId_GetCustIdFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal(" hello world", logEntry.CustId);

        }

        [Theory]
        [InlineData("Hello World")]
        public void CustId_GetNAFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal("N/A", logEntry.CustId);

        }

        [Theory]
        [InlineData("Message: The stored procedure 'NAP_Admin_VerifySiteName' doesn't exist.\r\nSOURCE: System.Data.\r\nDOMAIN: localhost.\r\nUrl: hello world\r\n")]
        public void SiteAddress_GetSiteAddressFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal(" hello world", logEntry.SiteAddress);

        }

        [Theory]
        [InlineData("Hello World")]
        public void SiteAddress_GetNAFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal("N/A", logEntry.SiteAddress);

        }

        [Theory]
        [InlineData("Message: The stored procedure 'NAP_Admin_VerifySiteName' doesn't exist.\r\nSOURCE: System.Data.\r\nDOMAIN: localhost.\r\nSeverity: hello world\r\n")]
        public void Severity_GetSeverityFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal(" hello world", logEntry.Severity);

        }

        [Theory]
        [InlineData("Hello World")]
        public void Severity_GetNAFromMessage_ReturnTrue(string message)
        {
            var logEntry = new LogEntry()
            {
                Message = message
            };

            Assert.Equal("N/A", logEntry.Severity);

        }

    }
}