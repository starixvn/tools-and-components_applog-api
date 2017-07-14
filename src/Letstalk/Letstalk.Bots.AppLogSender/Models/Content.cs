using System.Collections.Generic;

namespace Letstalk.Bots.AppLogSender.Models
{
    public interface IContent
    {
        Dictionary<string, string> Message { get; set; }

        List<string> Attachments { get; set; }

        List<string> InlineImages { get; set; }
    }
}