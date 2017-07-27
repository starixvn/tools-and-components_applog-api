using System;
using Letstalk.Bots.AppLogSender.Services;
using Xunit;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    [Trait("Service", "AppSettings")]
    public class AppSettingsTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData(null)]
        public void GetSettingValue_KeyIsNullOrWhiteSpace_ThrowArgumentException(string key)
        {
            Assert.Throws<ArgumentException>(() => AppSettings.GetSettingValue(key));            
        }
    }
}
