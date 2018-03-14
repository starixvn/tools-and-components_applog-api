using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Corona.Models;
using Skype.Bot.DataAccess;
using Skype.Bot.Services;

namespace Skype.Bot.Services
{
    public interface IConversationEventService
    {
        void Insert(ConversationEventModel conversationEvent);

        void Update(int conversationId, int eventId, ConversationEventModel conversationEvent);

        void Delete(int conversationId, int eventId);

        void DeleteConversation(int conversationId);

        bool Existing(int conversationId, int eventId);

        IList<ConversationEventModel> ConversationEvents { get; }               
    }
    public class ConversationEventService : QueryService, IConversationEventService
    {
        private ConversationEventModel Transform(ConversationEvent model)
        {
            return new ConversationEventModel
            {
                ConversationId = model.ConversationId,
                EventId = model.EventId,
                Name = model.Name,
                Disabled = model.Disabled
            };
        }

        public IList<ConversationEventModel> ConversationEvents
        {
            get
            {
                var conversationEvents = new List<ConversationEventModel>();

                Entities.ConversationEvents
                    .Where(x => !x.Disabled)
                    .ToList()
                    .ForEach(x =>
                    {
                        conversationEvents.Add(Transform(x));
                    });

                return conversationEvents;
            }
        }

        public void Delete(int conversationId, int eventId)
        {
            var entity = Entities.ConversationEvents.First(x => x.Id == conversationId && x.EventId == eventId);
            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Deleted;
                Entities.ConversationEvents.Remove(entity);
                Entities.SaveChanges();
            }
        }

        public void DeleteConversation(int conversationId)
        {
            var conversationEvents = Entities.ConversationEvents.Where(x => x.ConversationId == conversationId);
            if (conversationEvents.Any())
            {
                conversationEvents.ToList().ForEach(x => Entities.Entry(x).State = EntityState.Deleted);

                Entities.ConversationEvents.RemoveRange(conversationEvents);
                Entities.SaveChanges();
            }
        }

        public bool Existing(int conversationId, int eventId)
        {
            return Entities.ConversationEvents.Any(x => x.ConversationId == conversationId && x.EventId == eventId);
        }

        public void Insert(ConversationEventModel conversationEvent)
        {
            var entity = new ConversationEvent
            {
                ConversationId = conversationEvent.ConversationId,
                EventId = conversationEvent.EventId,
                Name = conversationEvent.Name,
                Disabled = conversationEvent.Disabled
            };
            Entities.Entry(entity).State = EntityState.Modified;
            Entities.ConversationEvents.Add(entity);

            Entities.SaveChanges();
        }

        public void Update(int conversationId, int eventId, ConversationEventModel conversationEvent)
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