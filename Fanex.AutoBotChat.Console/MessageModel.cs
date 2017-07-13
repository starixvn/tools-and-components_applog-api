using System.Collections.Generic;

namespace Fanex.AutoBotChat.Console
{
    class MessageModel
    {
        public string TargetName { get; set; }

        public Dictionary<string, string> Message { get; set; }

        public List<string> Attachments { get; set; }

        public List<string> InlineImages { get; set; }
    }
}
