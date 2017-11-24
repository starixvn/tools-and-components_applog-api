using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace Corona.Models
{
    [Serializable]
    public class ConversationEventSelectorModel
    {
        [Prompt("Your Conversation ID is: ")]
        public int ConversationId { get; set; }

        [Prompt("Your Event ID is: ")]
        public int EventId { get; set; }

        [Prompt("Your Event Name is: ")]
        public string Name { get; set; }
    }
}