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
            var service = new GroupManagerImpl(new StringDictionary());

            var expected = service.GetGroupByCategory(category);

            Assert.Empty(expected);
        }

        [Fact]
        public void GetGroupByCategory_CategoryGroupDictionaryDoesNotContainCategory_ReturnEmpty()
        {
            var categoryGroupDict = new StringDictionary
            {
                ["category 1"] = "group 1",
                ["category 2"] = "group 2"
            };

            var service = new GroupManagerImpl(categoryGroupDict);

            var expected = service.GetGroupByCategory("Anders' categtory");

            Assert.Empty(expected);
        }
    }
}