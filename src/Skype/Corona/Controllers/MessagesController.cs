using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using Corona.Models;
using Corona.Services;
using System.Collections;
using System.Collections.Generic;
using Corona.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Text;
using System.IO;

namespace Corona
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        private readonly IConversationService conversationService;
        private readonly IChannelAccountService channelAccountService;
        private readonly IConversationEventService conversationEventService;
        private readonly IList<string> HelloCollection;

        public MessagesController()
        {
            conversationService = new ConversationService();
            channelAccountService = new ChannelAccountService();
            conversationEventService = new ConversationEventService();
            HelloCollection = new List<string> {
                "Hi",
                "Hi Bot",
                "Hi Corona",
                "Hey"
            };
        }


        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {                    
                    activity.Text = CoreService.FormatMessage(activity.Text);
                                        
                    var isSayHello = HelloCollection.Any(x => x.Equals(activity.Text, StringComparison.OrdinalIgnoreCase));
                    if (isSayHello)
                    {
                        Activity reply = activity.CreateReply("Hi! " + activity.From.Name);
                        var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    await Conversation.SendAsync(activity, () => new RootDialog());
                }
                else
                {
                    HandleSystemMessage(activity);
                }

             
            }
            catch (Exception ex)
            {
                CoreService.WriteLog(ex.Message);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                if (message.MembersAdded.Any(x => x.Id == message.Recipient.Id))
                {                   
                    var replyMessage = new StringBuilder();

                    InsertConversation(message);                    

                    replyMessage.Append("Hi colleagues! \n\n");
                    replyMessage.Append("I'm Corona. Let's say 'Hi' or 'Help' to get the support from me. Thanks \n\n");

                    Activity reply = message.CreateReply(replyMessage.ToString());
                    var connector = new ConnectorClient(new Uri(message.ServiceUrl));

                    connector.Conversations.ReplyToActivityAsync(reply);
                }

                if (message.MembersRemoved.Any(x => x.Id == message.Recipient.Id))
                {
                    DeleteConversation(message);
                }

            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {

            }

            return null;
        }

        private void InsertConversation(Activity activity)
        {
            if (activity.Conversation.IsGroup.HasValue && activity.Conversation.IsGroup.Value)            
            {  
                var conversationModel = new ConversationModel
                {
                    ChannelId = activity.ChannelId,
                    Id = activity.Conversation.Id,
                    Name = activity.TopicName,
                    Recipient = new ChannelAccountModel
                    {
                        Id = activity.Recipient.Id,
                        Name = activity.Recipient.Name
                    },
                    Sender = new ChannelAccountModel
                    {
                        Id = activity.From.Id,
                        Name = activity.From.Name
                    },
                    IsGroup = activity.Conversation.IsGroup ?? false,
                    ServiceUrl = new Uri(activity.ServiceUrl)
                };

                var isExisting = conversationService.Existing(activity.Conversation.Id);

                if (!isExisting)
                {
                    conversationService.Insert(conversationModel);

                    if (!channelAccountService.Existing(conversationModel.Recipient.Id))
                    {
                        channelAccountService.Insert(new ChannelAccountModel { Id = conversationModel.Recipient.Id, Name = conversationModel.Recipient.Name });
                    }

                    if (!channelAccountService.Existing(conversationModel.Sender.Id))
                    {
                        channelAccountService.Insert(new ChannelAccountModel { Id = conversationModel.Sender.Id, Name = conversationModel.Sender.Name });
                    }

                    conversationService.UpdateMemberInfo(conversationModel);
                }
            }
        }

        private void DeleteConversation(Activity activity)
        {
            var isExisting = conversationService.Existing(activity.Conversation.Id);

            if (isExisting)
            {
                conversationService.Delete(activity.Conversation.Id);
                conversationEventService.DeleteConversation(activity.Conversation.Id);
            }
        }
    }
}