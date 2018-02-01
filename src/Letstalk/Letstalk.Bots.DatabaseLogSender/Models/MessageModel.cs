using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Letstalk.Bots.DatabaseLogSender.Models
{
   public class MessageModel
    {
        public int ConversationId { get; set; }

        public string Message { get; set; }
    }
}
