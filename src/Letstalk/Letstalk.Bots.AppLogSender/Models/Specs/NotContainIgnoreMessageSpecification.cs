using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Letstalk.Bots.AppLogSender.Models.Specs
{
    public class NotContainIgnoreMessageSpecification : ISpecification<LogEntry>
    {
        private readonly IEnumerable<string> ignoreTextList;

        public NotContainIgnoreMessageSpecification(IEnumerable<string> ignoreTextList)
        {
            this.ignoreTextList = ignoreTextList;
        }

        public ISpecification<LogEntry> And(ISpecification<LogEntry> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSatisfiedBy(LogEntry candidate)
            => candidate.IsValid && (ignoreTextList == null
                || !ignoreTextList.Any(ingoreText => candidate.Message.Contains(ingoreText)));

        public ISpecification<LogEntry> Not()
        {
            throw new NotImplementedException();
        }

        public ISpecification<LogEntry> Or(ISpecification<LogEntry> other)
        {
            throw new NotImplementedException();
        }
    }
}