using System.Collections.Immutable;
using System.Linq;

namespace Letstalk.Bots.AppLogSender.Models
{
    public class LogEntry
    {
        public string Message { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(Message);
    }
}