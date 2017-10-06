using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Corona.EntityModel;
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

        bool Resume(ConversationModel conversation, string message);

        IList<ConversationModel> Conversations { get; }

        ConversationModel Conversation(string id);

        void UpdateMemberInfo(ConversationModel conversation);

    }
    public class ConversationService : QueryService, IConversationService
    {
        readonly IChannelAccountService channelAccountService;

        public ConversationService()
        {
            channelAccountService = new ChannelAccountService();
        }
        public IList<ConversationModel> Conversations
        {
            get
            {
                if (Entities.Conversations.Any())
                {
                    var conversations = new List<ConversationModel>();
                    Entities.Conversations.ToList().ForEach(x =>
                    conversations.Add(new ConversationModel
                    {
                        Id = x.Id,
                        ChannelId = x.ChannelId,
                        IsGroup = x.IsGroup ?? false,
                        Name = x.Name,
                        Sender = channelAccountService.ChannelAccount(x.SenderId),
                        Recipient = channelAccountService.ChannelAccount(x.RecipientId),
                        ServiceUrl = new Uri(x.ServiceUrl)
                    }));

                    return conversations;
                }

                return new List<ConversationModel>();
            }
        }

        public ConversationModel Conversation(string id)
        {
            return Conversations.First(x => x.Id == id);
        }

        public void Delete(string id)
        {
            var entity = Entities.Conversations.Find(id);
            Entities.Entry(entity).State = EntityState.Deleted;
            Entities.Conversations.Remove(entity);
            Entities.SaveChanges();
        }

        public bool Existing(string id)
        {
            return Entities.Conversations.Any(x => x.Id == id);
        }

        public void Insert(ConversationModel conversation)
        {
            var entityConversation = new EntityModel.Conversation
            {
                ChannelId = conversation.ChannelId,
                Id = conversation.Id,
                IsGroup = conversation.IsGroup,
                Name = conversation.Name,
                RecipientId = conversation.Recipient.Id,
                SenderId = conversation.Sender.Id,
                ServiceUrl = conversation.ServiceUrl.ToString()
            };

            Entities.Entry(entityConversation).State = EntityState.Modified;
            Entities.Conversations.Add(entityConversation);
            Entities.SaveChanges();
        }

        public bool Resume(ConversationModel conversation, string message)
        {
            if (conversation.IsGroup)
            {
                var userAccount = new Microsoft.Bot.Connector.ChannelAccount
                {
                    Id = conversation.Sender.Id,
                    Name = conversation.Sender.Name
                };

                var botAccount = new Microsoft.Bot.Connector.ChannelAccount
                {
                    Id = conversation.Recipient.Id,
                    Name = conversation.Recipient.Name
                };

                var connector = new ConnectorClient(conversation.ServiceUrl);
                IMessageActivity activityMessage = Activity.CreateMessageActivity();

                activityMessage.ChannelId = conversation.ChannelId;
                activityMessage.From = botAccount;
                activityMessage.Recipient = userAccount;
                activityMessage.Conversation = new ConversationAccount(id: conversation.Id);
                activityMessage.Text = message;
                activityMessage.Locale = "en-Us";

                Microsoft​App​Credentials.TrustServiceUrl(conversation.ServiceUrl.ToString());

                var result = connector.Conversations.SendToConversationAsync((Activity)activityMessage);

                return result.Exception == null;
            }

            return false;
        }

        public void Update(string id, ConversationModel conversation)
        {
            var entity = Entities.Conversations.Find(id);

            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Modified;

                entity.ChannelId = conversation.ChannelId;
                entity.IsGroup = conversation.IsGroup;
                entity.RecipientId = conversation.Recipient.Id;
                entity.SenderId = conversation.Sender.Id;
                entity.Name = conversation.Name;
                entity.ServiceUrl = conversation.ServiceUrl.ToString();

                Entities.SaveChanges();
            }
        }

        public void UpdateMemberInfo(ConversationModel conversation)
        {
            var connector = new ConnectorClient(conversation.ServiceUrl);
            var conversationMembers = connector.Conversations.GetConversationMembers(conversation.Id);
            
            foreach (var conMember in conversationMembers)
            {
                CoreService.WriteLog(conMember.Id + " - " + conMember.Name);
                channelAccountService.Update(conMember.Id, new ChannelAccountModel { Name = conMember.Name });
            }
        }
    }
}