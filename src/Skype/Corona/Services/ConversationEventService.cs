using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corona.Models;

namespace Corona.Services
{
    public interface IConversationEventService
    {
        void Insert(ConversationEventModel conversationEvent);

        void Update(string conversationId, string eventId, ConversationEventModel conversationEvent);

        void Delete(string conversationId, string eventId);

        bool Existing(string conversationId, string eventId);

        IList<ConversationEventModel> ConversationEvents { get; }
    }
    public class ConversationEventService : BaseService, IConversationEventService
    {
        public IList<ConversationEventModel> ConversationEvents
        {
            get
            {
                return entities.ConversationEvents.Select(x => new ConversationEventModel
                {
                    ConversationId = x.ConversationId,
                    EventId = x.EventId,
                    Name = x.Name
                }).ToList();
            }           
        }

        public void Delete(string conversationId, string eventId)
        {
            var entity = entities.ConversationEvents.Where(x => x.ConversationId == conversationId && x.EventId == eventId).First();

            entities.ConversationEvents.Remove(entity);
            entities.SaveChanges();
        }

        public bool Existing(string conversationId, string eventId)
        {
            var entity = entities.ConversationEvents.Where(x => x.ConversationId == conversationId && x.EventId == eventId).First();

            return entity != null;
        }

        public void Insert(ConversationEventModel conversationEvent)
        {
            entities.ConversationEvents.Add(
                new EntityModel.ConversationEvent
                {
                   ConversationId = conversationEvent.ConversationId,
                   EventId = conversationEvent.EventId,
                   Name = conversationEvent.Name
                });

            entities.SaveChanges();
        }

        public void Update(string conversationId, string eventId, ConversationEventModel conversationEvent)
        {
            var entity = entities.ConversationEvents.Where(x => x.ConversationId == conversationId && x.EventId == eventId).First();

            entity.Name = conversationEvent.Name;
            entities.SaveChanges();                
        }
    }
}