using Letstalk.Bots.AppLogSender.Services;
using System;
using Xunit;
using NSubstitute;
using Fanex.Data.Repository;

namespace Letstalk.Bots.AppLogSender.UnitTests.Services
{
    [Trait("Service", "LogDataService")]
    public class LogDataServiceImplTests
    {
        //[Fact]
        //public void GetLatestData_DoestNotContainIgnoredMessages()
        //{
        //    var ignoreTextService = Substitute.For<ContentValidator>().GetAll();
        //    var repository = Substitute.For<IDynamicRepository>();
        //    var logDataservice = new LogDataServiceImpl(ignoreTextService, repository);
        //}
    }
}