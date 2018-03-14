using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Corona.Models;
using Microsoft.Bot.Connector;
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
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                if (message.MembersAdded.Any(x => x.Id == message.Recipient.Id))
                {
                    var replyMessage = new StringBuilder();

                    conversationService.Insert(message);

                    replyMessage.Append("Hi! \n\n");
                    replyMessage.Append("My name is Corona. I'm very happy to chat with you. Thanks \n\n");

                    Activity reply = message.CreateReply(replyMessage.ToString());
                    var connector = new ConnectorClient(new Uri(message.ServiceUrl));

                    connector.Conversations.ReplyToActivityAsync(reply);
                }

                if (message.MembersRemoved.Any(x => x.Id == message.Recipient.Id))
                {
                    conversationService.Delete(message);
                }
            }
        }

        private static string FormatMessage(string message) => message.Replace("@Corona", string.Empty).Trim();
    }
}