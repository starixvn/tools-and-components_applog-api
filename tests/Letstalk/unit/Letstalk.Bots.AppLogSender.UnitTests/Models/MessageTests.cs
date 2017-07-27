using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Letstalk.Bots.AppLogSender.Models;
using Xunit;


namespace Letstalk.Bots.AppLogSender.UnitTests.Models
{
    [Trait("Model", "Message")]
    public class MessageTests
    {
        [Fact]
        public void Message_LogEntryIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Message(null));
        }
    }
}
