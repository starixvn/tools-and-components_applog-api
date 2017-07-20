using System.Collections.Specialized;
using Letstalk.Bots.AppLogSender.Services;
using Xunit;

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
    }
}