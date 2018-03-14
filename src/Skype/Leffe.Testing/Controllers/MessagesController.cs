using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using Corona.Models;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Text;
using System.IO;
using Skype.Bot.Services;

namespace Leffe
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        private readonly IConversationService conversationService;
        private readonly IChannelAccountService channelAccountService;
        private readonly IConversationEventService conversationEventService;
        private readonly IUserMessageService userMessageService;

        public MessagesController()
        {
            conversationService = new ConversationService();
            channelAccountService = new ChannelAccountService();
            conversationEventService = new ConversationEventService();
            userMessageService = new UserMessageService();
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                var userMessages = userMessageService.GetMessage();

                foreach (var userMessage in userMessages)
                {
                    switch (userMessage.Key)
                    {
                        case UserMessageKeyModel.GetConversationId:
                            {
                                if (userMessage.Value.Contains(FormatMessage(activity.Text), StringComparer.CurrentCultureIgnoreCase))
                                {
                                    conversationService.GetConversationId(activity);
                                }

                                break;
                            }
                        case UserMessageKeyModel.SayHi:
                            {
                                if (userMessage.Value.Contains(FormatMessage(activity.Text), StringComparer.CurrentCultureIgnoreCase))
                                {
                                    Activity reply = activity.CreateReply("Hi! " + activity.From.Name);

                                    await connector.Conversations.ReplyToActivityAsync(reply);
                                }

                                break;
                            }
                    }
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private void HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.ConversationUpdate)
            {
                if (message.MembersAdded.Any(x => x.Id == message.Recipient.Id))
                {                    
                    conversationService.Insert(message);
                }

                //if (message.MembersRemoved.Any(x => x.Id == message.Recipient.Id))
                //{
                //    conversationService.Delete(message);
                //}
            }
        }

        private static string FormatMessage(string message) => message.Replace("@leffe", string.Empty).Trim();
    }
}