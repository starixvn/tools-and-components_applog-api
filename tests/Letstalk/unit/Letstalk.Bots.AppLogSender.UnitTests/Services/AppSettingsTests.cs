using Letstalk.Bots.AppLogSender.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void GetSettingValue_IsNullOrWhiteSpace_ThrowArgumentException(string key)
        {
            Assert.Throws<ArgumentException>(() => AppSettings.GetSettingValue(key));            
        }
    }
    
}
