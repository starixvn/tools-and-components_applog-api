using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Corona.Models
{
    public class ConversationEventModel
    {
        public int ConversationId { get; set; }

        public int EventId { get; set; }

        public string Name { get; set; }

        public bool Disabled { get; set; }
    }
}