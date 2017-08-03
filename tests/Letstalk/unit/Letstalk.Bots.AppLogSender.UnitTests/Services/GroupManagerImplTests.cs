using Letstalk.Bots.AppLogSender.Services;
using Xunit;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    [Trait("Service", "GroupManager")]
    public class GroupManagerImplTests
    {
        private const string JsonContentCategoryGroup = "{'category 1': 'Group A', 'LA': 'Sparta-BOTCHAT-ErrorLog'}";  
    
        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData(null)]
        public void GetGroupByCategory_CategoryIsNullOrEmpty_ReturnEmpty(string category)
        {
            var service = new GroupManagerImpl(JsonContentCategoryGroup);

            var expected = service.GetGroupByCategory(category);

            Assert.Empty(expected);
        }

        [Fact]
        public void GetGroupByCategory_CategoryGroupDictionaryDoesNotContainCategory_ReturnEmpty()
        {
            var service = new GroupManagerImpl(JsonContentCategoryGroup);

            var expected = service.GetGroupByCategory("Anders' categtory");

            Assert.Empty(expected);
        }
    }
}