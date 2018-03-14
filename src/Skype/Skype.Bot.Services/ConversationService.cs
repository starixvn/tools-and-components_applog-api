using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Corona.Models;
using Microsoft.Bot.Connector;
using Skype.Bot.DataAccess;
using Skype.Bot.Services;

namespace Skype.Bot.Services
{
    public interface IConversationService
    {
        int Insert(ConversationModel conversation);

        void Insert(Activity activity);

        void Update(int id, ConversationModel conversation);

        void Delete(int id);

        void Delete(Activity activity);

        bool ExistingConversation(string conversationId);

        bool Existing(int id);

        bool Resume(ConversationModel conversation, string message);

        IList<ConversationModel> Conversations { get; }

        ConversationModel Conversation(int id);

        ConversationModel Conversation(string conversationId);

        void UpdateMemberInfo(ConversationModel conversation);

        void GetConversationId(Activity activity);

    }
    public class ConversationService : QueryService, IConversationService
    {
        readonly IChannelAccountService channelAccountService;

        readonly IConversationEventService conversationEventService;

        readonly IEventService eventService;

        public ConversationService()
        {
            channelAccountService = new ChannelAccountService();
            conversationEventService = new ConversationEventService();
            eventService = new EventService();
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
                        ConversationId = x.ConversationId,
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

        public ConversationModel Conversation(string conversationId)
        {
            if (Conversations.Any(x => x.ConversationId == conversationId))
            {
                return Conversations.First(x => x.ConversationId == conversationId);
            }

            return null;
        }

        public ConversationModel Conversation(int id)
        {
            return Conversations.First(x => x.Id == id);
        }

        public void Delete(Activity activity)
        {
            var conversation = Conversation(activity.Conversation.Id);

            if (conversation != null && conversation.Id != 0)
            {
                Delete(conversation.Id);
                conversationEventService.DeleteConversation(conversation.Id);
            }
        }

        public void Delete(int id)
        {
            var entity = Entities.Conversations.Find(id);
            Entities.Entry(entity).State = EntityState.Deleted;
            Entities.Conversations.Remove(entity);
            Entities.SaveChanges();
        }

        public bool Existing(int id)
        {
            return Entities.Conversations.Any(x => x.Id == id);
        }

        public bool ExistingConversation(string conversationId)
        {
            return Entities.Conversations.Any(x => x.ConversationId == conversationId);
        }

        public void GetConversationId(Activity activity)
        {
            string message = "Sorry, this is not the Skype group. Please try again. Thanks.";

            if (activity.Conversation.IsGroup.HasValue && activity.Conversation.IsGroup.Value)
            {
                var conService = new ConversationService();

                var conversations = conService.Conversations;

                if (conversations.Any())
                {
                    var id = conversations.First(x => x.ConversationId == activity.Conversation.Id).Id;

                    message = "The group id is " + id.ToString();
                }
            }

            Activity reply = activity.CreateReply(message);
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            connector.Conversations.ReplyToActivityAsync(reply).ConfigureAwait(false);
        }

        public void Insert(Activity activity)
        {
            if (activity.Conversation.IsGroup.HasValue && activity.Conversation.IsGroup.Value)
            {
                var conversationModel = new ConversationModel
                {
                    ChannelId = activity.ChannelId,
                    ConversationId = activity.Conversation.Id,
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

                var isExisting = ExistingConversation(activity.Conversation.Id);

                if (!isExisting)
                {
                    var id = Insert(conversationModel);
                    var events = eventService.EventsList().Select(x => new { Id = x.Id, Name = x.Name });

                    foreach (var myEvent in events)
                    {
                        conversationEventService.Insert(new ConversationEventModel
                        {
                            ConversationId = id,
                            Disabled = true,
                            EventId = myEvent.Id,
                            Name = myEvent.Name
                        });
                    }

                    if (!channelAccountService.Existing(conversationModel.Recipient.Id))
                    {
                        channelAccountService.Insert(new ChannelAccountModel { Id = conversationModel.Recipient.Id, Name = conversationModel.Recipient.Name });
                    }

                    if (!channelAccountService.Existing(conversationModel.Sender.Id))
                    {
                        channelAccountService.Insert(new ChannelAccountModel { Id = conversationModel.Sender.Id, Name = conversationModel.Sender.Name });
                    }

                    UpdateMemberInfo(conversationModel);
                }
            }
        }

        public int Insert(ConversationModel conversation)
        {
            var entityConversation = new Conversation
            {
                ChannelId = conversation.ChannelId,
                ConversationId = conversation.ConversationId,
                IsGroup = conversation.IsGroup,
                Name = conversation.Name,
                RecipientId = conversation.Recipient.Id,
                SenderId = conversation.Sender.Id,
                ServiceUrl = conversation.ServiceUrl.ToString()
            };

            Entities.Entry(entityConversation).State = EntityState.Modified;
            Entities.Conversations.Add(entityConversation);           
            Entities.SaveChanges();

            return entityConversation.Id;
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
                activityMessage.Conversation = new ConversationAccount(id: conversation.ConversationId);
                activityMessage.Text = message;
                activityMessage.Locale = "en-Us";
                activityMessage.TextFormat = TextFormatTypes.Xml;
                Microsoft​App​Credentials.TrustServiceUrl(conversation.ServiceUrl.ToString());

                var result = connector.Conversations.SendToConversationAsync((Activity)activityMessage);

                return result.Exception == null;
            }

            return false;
        }

        public void Update(int id, ConversationModel conversation)
        {
            var entity = Entities.Conversations.Find(id);

            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Modified;

                entity.ChannelId = conversation.ChannelId;
                entity.ConversationId = conversation.ConversationId;
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
            var conversationMembers = connector.Conversations.GetConversationMembers(conversation.ConversationId);

            foreach (var conMember in conversationMembers)
            {
                channelAccountService.Update(conMember.Id, new ChannelAccountModel { Name = conMember.Name });
            }
        }
    }
}