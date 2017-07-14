using System;
using System.Collections.Generic;
using System.Linq;
using Letstalk.Bots.AppLogSender.Models;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface ContentValidator
    {
        bool IsValid(LogEntry entry);
    }

    public class ContentValidatorImpl : ContentValidator
    {
        private readonly IEnumerable<string> ignoreTextList;

        public ContentValidatorImpl(IEnumerable<string> ignoreTextList = null)
        {
            this.ignoreTextList = ignoreTextList ?? GetAll();
        }

        public bool IsValid(LogEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            return entry.IsValid
                && (ignoreTextList == null
                || !ignoreTextList.Any(ingoreText => entry.Message.Contains(ingoreText)));
        }

        private IEnumerable<string> GetAll()
        {
            // TODO: Terry will load data from config file here
            return Enumerable.Empty<string>();
        }
    }
}