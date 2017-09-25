using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Corona.EntityModel;
using Corona.Models;

namespace Corona.Services
{
    public interface IConversationEventService
    {
        void Insert(ConversationEventModel conversationEvent);

        void Update(string conversationId, int eventId, ConversationEventModel conversationEvent);

        void Delete(string conversationId, int eventId);

        void DeleteConversation(string conversationId);

        bool Existing(string conversationId, int eventId);

        IList<ConversationEventModel> ConversationEvents { get; }
    }
    public class ConversationEventService : QueryService, IConversationEventService
    {
        public IList<ConversationEventModel> ConversationEvents
        {
            get
            {
                return Entities.ConversationEvents.Select(x => new ConversationEventModel
                {
                    ConversationId = x.ConversationId,
                    EventId = x.EventId,
                    Name = x.Name
                }).ToList();
            }
        }

        public void Delete(string conversationId, int eventId)
        {
            var entity = Entities.ConversationEvents.First(x => x.ConversationId == conversationId && x.EventId == eventId);
            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Deleted;
                Entities.ConversationEvents.Remove(entity);
                Entities.SaveChanges();
            }
        }

        public void DeleteConversation(string conversationId)
        {
            var conversationEvents = Entities.ConversationEvents.Where(x => x.ConversationId == conversationId);
            if (conversationEvents.Any())
            {
                conversationEvents.ToList().ForEach(x => Entities.Entry(x).State = EntityState.Deleted);

                Entities.ConversationEvents.RemoveRange(conversationEvents);
                Entities.SaveChanges();
            }
        }

        public bool Existing(string conversationId, int eventId)
        {
            return Entities.ConversationEvents.Any(x => x.ConversationId == conversationId && x.EventId == eventId);
        }

        public void Insert(ConversationEventModel conversationEvent)
        {
            var entity = new EntityModel.ConversationEvent
            {
                ConversationId = conversationEvent.ConversationId,
                EventId = conversationEvent.EventId,
                Name = conversationEvent.Name
            };
            Entities.Entry(entity).State = EntityState.Modified;
            Entities.ConversationEvents.Add(entity);

            Entities.SaveChanges();
        }

        public void Update(string conversationId, int eventId, ConversationEventModel conversationEvent)
        {
            var entity = Entities.ConversationEvents.Where(x => x.ConversationId == conversationId && x.EventId == eventId).First();

            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Modified;
                entity.Name = conversationEvent.Name;
                Entities.SaveChanges(); 
            }
        }
    }
}