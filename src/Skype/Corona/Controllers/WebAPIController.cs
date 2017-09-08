using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Corona.Services;

namespace Corona.Controllers
{
    public class WebAPIController : ApiController
    {
        readonly IConversationService conversationService;
        public WebAPIController() {
            conversationService = new ConversationService();
        }

        [HttpGet]
        [HttpPost]
        [Route("api/AutoBotWebAPI")]
        public async Task<HttpResponseMessage> SendMessage(string message)
        {
            try
            {
                //var conversations = conversationService.

                //if (conversations.Count != 0)
                //{
                //    foreach (var conversation in conversations)
                //    {
                //        await ConversationSenderService.Resume(conversation, message);
                //    }
                //}

                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                resp.Content = new StringContent("Message is sent at " + DateTime.Now);

                return resp;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

    }
}