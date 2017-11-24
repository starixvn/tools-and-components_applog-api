using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Corona.Models
{
    public class ConversationModel
    {
        public int Id { get; set; }

        public string ConversationId { get; set; }

        public string Name { get; set; }

        public ChannelAccountModel Sender { get; set; }                

        public ChannelAccountModel Recipient { get; set; }

        public Uri ServiceUrl { get; set; }

        public bool  IsGroup { get; set; }

        public string ChannelId { get; set; }
    }
}