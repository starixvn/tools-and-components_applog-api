using System;
using System.Collections.Specialized;
using Letstalk.Bots.AppLogSender.Services;
using Xunit;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    [Trait("Service", "AppSettings")]
    public class AppSettingsTests
    {
        private const string CurrentBaseDir = "Current/BaseDir";               

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData(null)]
        public void GetSettingValue_KeyIsNullOrWhiteSpace_ThrowArgumentException(string key)
        {
            Assert.Throws<ArgumentException>(() => AppSettings.GetSettingValue(key));            
        }

        [Theory]        
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void IsProduction_Always_ReturnRightValueGetFromKeyisProduction(string value, bool expected)
        {
            var keyValues = new NameValueCollection
            {
                { "isProduction", value }
            };

            AppSettings.SetupGetConfiValueFunction(keyValues);

            var actual = AppSettings.IsProduction;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void IsProduction_Always_ReturnRightValueFromSetting(string value, bool expected)
        {
            var keyValues = new NameValueCollection
            {
                { "isProduction", value }
            };

            AppSettings.SetupGetConfiValueFunction(keyValues);

            var actual = AppSettings.IsProduction;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        public void Interval_Always_ReturnRightValueFromSetting(int expected)
        {            
            var keyValues = new NameValueCollection
            {
                { "interval", expected.ToString() }
            };

            AppSettings.SetupGetConfiValueFunction(keyValues);

            var actual = AppSettings.Interval;

            Assert.Equal(expected, actual);
        }
    }
}
