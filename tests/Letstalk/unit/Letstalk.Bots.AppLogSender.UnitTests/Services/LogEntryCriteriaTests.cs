using System;
using Letstalk.Bots.AppLogSender.Services;
using Xunit;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    [Trait("Service", "LogEntryCriteria")]
    public class LogEntryCriteriaTests
    {
        private LogEntryCriteria logEntryCriteria = new LogEntryCriteria(DateTime.Now);

        [Theory]
        [InlineData("IS_SelAppLog_All")]
        public void GetSettingKey_IS_SelAppLog_All_ReturnTrue(string settingKey)
        {
            Assert.True(logEntryCriteria.GetSettingKey().Equals(settingKey));
        }

        [Fact]
        public void IsValid_ReturnTrue()
        {
            Assert.True(logEntryCriteria.IsValid());
        }
    }
}
