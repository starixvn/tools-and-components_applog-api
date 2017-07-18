using Letstalk.Bots.AppLogSender.Services;
using System;
using Xunit;
using NSubstitute;
using Fanex.Data.Repository;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    public class GroupServiceImplTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetGroupByCategory_CategoryIsNullOrEmpty_ReturnEmpty(string category)
        {
            var service = new GroupServiceImpl();

            var expected = service.GetGroupByCategory(category);

            Assert.Empty(expected);
        }
    }
}