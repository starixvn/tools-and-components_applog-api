using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Letstalk.Bots.AppLogSender.Services;

namespace Letstalk.Bots.AppLogSender.Controllers
{
    public class AppLogSenderController
    {
        private readonly GroupMapper _groupMapper;
        private readonly LogEntryChecker _logEntryChecker;
        private readonly LogContentGetter _logContentGetter;
        private readonly MessageSender _messageSender;

        public AppLogSenderController()
        {
            _groupMapper = _groupMapper ?? new GroupMapperImpl();
            _logEntryChecker = _logEntryChecker ?? new LogEntryCheckerImpl();
            _logContentGetter = _logContentGetter ?? new LogContentGetterImpl();
            _messageSender = _messageSender ?? new MessageSenderImpl();
        }

        public void Send()
        {
            var contents = _logContentGetter.GetLatestData();
            foreach (var content in contents)
            {
                _messageSender.Send(content).To(content.Group);
            }
        }
    }
}
