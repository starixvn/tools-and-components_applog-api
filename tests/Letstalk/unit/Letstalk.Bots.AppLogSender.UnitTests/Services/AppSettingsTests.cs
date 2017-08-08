using System;
using System.Collections.Specialized;
using Letstalk.Bots.AppLogSender.Services;
using Xunit;
using System.IO;

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

            AppSettings.SetupConfigurationValues(keyValues);

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

            AppSettings.SetupConfigurationValues(keyValues);

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

            AppSettings.SetupConfigurationValues(keyValues);

            var actual = AppSettings.Interval;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("d7la9omvq6hte3oqtq7e")]
        public void AuthKey_Always_ReturnRightValueFromSetting(string expected)
        {
            var keyValues = new NameValueCollection
            {
                { "letstalk:AuthKey", expected }
            };

            AppSettings.SetupConfigurationValues(keyValues);
            var actual = AppSettings.AuthKey;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("http://10.40.40.140/NotificationService/")]
        public void ServiceUri_Always_ReturnRightValueFromSetting(string expected)
        {
            var keyValues = new NameValueCollection
            {
                { "letstalk:Service", expected }
            };

            AppSettings.SetupConfigurationValues(keyValues);

            var actual = AppSettings.ServiceUri;

            Assert.Equal(expected.Split(new[] { ',', ';' }), actual);
        }

        [Theory]
        [InlineData("my path", "Configurations\\CategoryGroupMapper.json")]
        public void CategoryGroupMapperFilePath_Always_ReturnRightValueFromSetting(string currentBaseDirectory, string expected)
        {
            var keyValues = new NameValueCollection
            {
                { "bot:CategoryGroupMapperPath", expected }
            };

            AppSettings.SetupConfigurationValues(keyValues);
            AppSettings.SetupCurrentBaseDirectory(currentBaseDirectory);

            var actual = AppSettings.CategoryGroupMapperFilePath;

            Assert.Equal(Path.Combine(currentBaseDirectory, expected), actual);
        }

        [Theory]
        [InlineData("", "Configurations\\CategoryGroupMapper.json")]
        public void CategoryGroupMapperFilePath_Always_ReturnRightValueFromSettingAndCurrentDomainPath(string currentBaseDirectory, string expected)
        {
            var keyValues = new NameValueCollection
            {
                { "bot:CategoryGroupMapperPath", expected }
            };

            AppSettings.SetupConfigurationValues(keyValues);
            AppSettings.SetupCurrentBaseDirectory(currentBaseDirectory);

            var actual = AppSettings.CategoryGroupMapperFilePath;

            Assert.Equal(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, expected), actual);
        }


        [Theory]
        [InlineData("my path", "Configurations\\SkipTheseMessages.txt")]
        public void IgnoreMessagesFilePath_Always_ReturnRightValueFromSetting(string currentBaseDirectory, string expected)
        {
            var keyValues = new NameValueCollection
            {
                { "bot:SkipTheseMessagesPath", expected }
            };

            AppSettings.SetupConfigurationValues(keyValues);
            AppSettings.SetupCurrentBaseDirectory(currentBaseDirectory);

            var actual = AppSettings.IgnoreMessagesFilePath;

            Assert.Equal(Path.Combine(currentBaseDirectory, expected), actual);
        }

        [Theory]
        [InlineData("", "Configurations\\SkipTheseMessages.txt")]
        public void IgnoreMessagesFilePath_Always_ReturnRightValueFromSettingAndCurrentDomainPath(string currentBaseDirectory, string expected)
        {
            var keyValues = new NameValueCollection
            {
                { "bot:SkipTheseMessagesPath", expected }
            };

            AppSettings.SetupConfigurationValues(keyValues);
            AppSettings.SetupCurrentBaseDirectory(currentBaseDirectory);

            var actual = AppSettings.IgnoreMessagesFilePath;

            Assert.Equal(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, expected), actual);
        }

        [Theory]
        [InlineData("~/App_Data")]
        public void FanexDbSettingPath_Always_ReturnRightValueFromSetting(string expected)
        {
            var keyValues = new NameValueCollection
            {
                { "fanex:dbsettingpath", expected }
            };

            AppSettings.SetupConfigurationValues(keyValues);            

            var actual = AppSettings.FanexDbSettingPath;

            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Verify_AuthKey_MissingKey_ReturnInvalidDataException()
        {
            var keyValues = new NameValueCollection
            {
                { "letstalk:AuthKey1", string.Empty },                
            };

            AppSettings.SetupConfigurationValues(keyValues);

            var authKey = AppSettings.AuthKey;                       

            Assert.Throws<InvalidDataException>(()=> AppSettings.Verify());            
        }


        [Fact]
        public void Verify_ServiceUri_MissingKey_ReturnInvalidDataException()
        {
            var keyValues = new NameValueCollection
            {
                { "letstalk:AuthKey", "xyz" },
                { "letstalk:Service", string.Empty }
            };

            AppSettings.SetupConfigurationValues(keyValues);

            var authKey = AppSettings.AuthKey;
            var serviceURL = AppSettings.ServiceUri;

            Assert.Throws<InvalidDataException>(() => AppSettings.Verify());            
        }

        [Fact]
        public void Verify_CategoryGroupMapperFilePath_NotFoundPath_ReturnFileNotFoundException()
        {
            var keyValues = new NameValueCollection
            {
                { "letstalk:AuthKey", "xyz" },
                { "letstalk:Service", "abc" },
                { "bot:CategoryGroupMapperPath", string.Empty },
            };

            AppSettings.SetupConfigurationValues(keyValues);
            AppSettings.SetupCurrentBaseDirectory();

            var authKey = AppSettings.AuthKey;
            var serviceURL = AppSettings.ServiceUri;
            var categoryGroupMapperPath = AppSettings.CategoryGroupMapperFilePath;

            Assert.Throws<FileNotFoundException>(() => AppSettings.Verify());
        }

        [Fact]
        public void Verify_IgnoreMessagesFilePath_NotFoundPath_ReturnFileNotFoundException()
        {
            var keyValues = new NameValueCollection
            {
                { "letstalk:AuthKey", "xyz" },
                { "letstalk:Service", "abc" },
                { "bot:CategoryGroupMapperPath", "Configurations\\CategoryGroupMapper.json" },
                { "bot:SkipTheseMessagesPath", string.Empty }
            };

            AppSettings.SetupConfigurationValues(keyValues);
            AppSettings.SetupCurrentBaseDirectory();

            var authKey = AppSettings.AuthKey;
            var serviceURL = AppSettings.ServiceUri;
            var categoryGroupMapperPath = AppSettings.CategoryGroupMapperFilePath;
            var skipTheseMessagesPath = AppSettings.IgnoreMessagesFilePath;

            Assert.Throws<FileNotFoundException>(() => AppSettings.Verify());
        }
    }
}
