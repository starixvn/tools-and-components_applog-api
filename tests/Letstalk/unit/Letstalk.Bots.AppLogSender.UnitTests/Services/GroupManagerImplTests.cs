using System.Collections.Specialized;
using Letstalk.Bots.AppLogSender.Services;
using Xunit;
using System.Collections.Generic;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    [Trait("Service", "GroupManager")]
    public class GroupManagerImplTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData(null)]
        public void GetGroupByCategory_CategoryIsNullOrEmpty_ReturnEmpty(string category)
        {
            var service = new GroupManagerImpl(new Dictionary<string, string>());

            var expected = service.GetGroupByCategory(category);

            Assert.Empty(expected);
        }

        [Theory]
        [InlineData("category 1")]
        public void GroupCategoryMapper_GetGroupNameByCategory_ReturnGroupName(string category)
        {
            var service = new GroupManagerImpl(null);
            var expected = service.GetGroupByCategory(category);

            Assert.True(expected.Equals("Group A"));

        }
    }
}