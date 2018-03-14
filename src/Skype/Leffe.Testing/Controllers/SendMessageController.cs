using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Corona.Models;
using Leffe.Filters;
using Skype.Bot.Services;

namespace Leffe
{
    [HMACAuthenticationAttribute]    
    [RoutePrefix("api/SendMessage")]
    public class SendMessageController : ApiController
    {
        readonly IConversationService conversationService;

        public SendMessageController()
        {
            conversationService = new ConversationService();            
        }

        [Route("")]
        public IHttpActionResult Post(MessageModel messageModel)
        {
            var conversationModel = conversationService.Conversation(messageModel.ConversationId);

            if (conversationModel != null)
            {
                var result = conversationService.Resume(conversationModel, messageModel.Message);

                return Ok<bool>(result);
            }

            return Ok<bool>(false);
        }
    }
}
