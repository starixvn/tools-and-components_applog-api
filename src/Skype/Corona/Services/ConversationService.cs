using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Corona.Models;
using Microsoft.Bot.Connector;

namespace Corona.Services
{
    public interface IConversationService
    {
        void Insert(ConversationModel conversation);

        void Update(string id, ConversationModel conversation);

        void Delete(string id);

        bool Existing(string id);

        Task Resume(ConversationModel conversation, string message);

        IList<ConversationModel> Conversations { get; }

    }
    public class ConversationService : BaseService, IConversationService
    {
        ChannelAccountService channelAccountService;

        public ConversationService() {
            channelAccountService = new ChannelAccountService();
        }
        public IList<ConversationModel> Conversations
        {
            get
            {
                return entities.Conversations.Select(x => new ConversationModel
                {
                    Id = x.Id,
                    ChannelId = x.ChannelId,
                    IsGroup = x.IsGroup.Value,
                    Name = x.Name,
                    Sender = channelAccountService.ChannelAccount(x.SenderId),
                    Recipient = channelAccountService.ChannelAccount(x.RecipientId),
                    ServiceUrl = new Uri(x.ServiceUrl)
                }).ToList();
            }
        }

        public void Delete(string id)
        {
            var entity = entities.Conversations.Find(id);

            entities.Conversations.Remove(entity);
            entities.SaveChanges();
        }

        public bool Existing(string id)
        {
            return entities.Conversations.Any(x => x.Id == id);            
        }

        public void Insert(ConversationModel conversation)
        {
            entities.Conversations.Add(
                new EntityModel.Conversation
                {
                    ChannelId = conversation.ChannelId,
                    Id = conversation.Id,
                    IsGroup = conversation.IsGroup,
                    Name = conversation.Name,
                    RecipientId = conversation.Recipient.Id,
                    SenderId = conversation.Sender.Id,
                    ServiceUrl = conversation.ServiceUrl.ToString()
                });                        

            entities.SaveChanges();
        }

        public async Task Resume(ConversationModel conversation, string message)
        {
            if (conversation.IsGroup)
            {
                var userAccount = new ChannelAccount
                {
                    Id = conversation.Sender.Id,
                    Name = conversation.Sender.Name
                };

                var botAccount = new ChannelAccount
                {
                    Id = conversation.Recipient.Id,
                    Name = conversation.Recipient.Name
                };

                var connector = new ConnectorClient(conversation.ServiceUrl);
                IMessageActivity activityMessage = Activity.CreateMessageActivity();

                if (!string.IsNullOrEmpty(conversation.Id) && !string.IsNullOrEmpty(conversation.ChannelId))
                {
                    activityMessage.ChannelId = conversation.ChannelId;
                }
                else
                {
                    conversation.Id = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
                }

                activityMessage.From = botAccount;
                activityMessage.Recipient = userAccount;
                activityMessage.Conversation = new ConversationAccount(id: conversation.Id);
                activityMessage.Text = message;
                activityMessage.Locale = "en-Us";

                Microsoft​App​Credentials.TrustServiceUrl(conversation.ServiceUrl.ToString());

                await connector.Conversations.SendToConversationAsync((Activity)activityMessage);
            }
        }

        public void Update(string id, ConversationModel conversation)
        {
            var entity = entities.Conversations.Find(id);

            entity.ChannelId = conversation.ChannelId;
            entity.IsGroup = conversation.IsGroup;
            entity.RecipientId = conversation.Recipient.Id;
            entity.SenderId = conversation.Sender.Id;
            entity.Name = conversation.Name;
            entity.ServiceUrl = conversation.ServiceUrl.ToString();

            entities.SaveChanges();
        }


    }
}