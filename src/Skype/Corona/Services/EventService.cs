using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corona.Models;

namespace Corona.Services
{
    public interface IEventService
    {
        void Insert(EventModel model);

        void Update(int id, EventModel model);

        void Delete(int id);

        void UpdateEvent(int id, DateTime start, DateTime end);
    }
    public class EventService : BaseService, IEventService
    {
        public void Delete(int id)
        {
            var entity = entities.Events.Find(id);

            entities.Events.Remove(entity);
            entities.SaveChanges();
        }

        public void Insert(EventModel model)
        {
            entities.Events.Add(new EntityModel.Event
            {
                Start = model.Start,
                End = model.End,
                Message = model.Message
            });

            entities.SaveChanges();
        }

        public void Update(int id, EventModel model)
        {
            var entity = entities.Events.Find(id);

            entity.Start = model.Start;
            entity.End = model.End;            
            entity.Message = model.Message;

            entities.SaveChanges();
        }

        public void UpdateEvent(int id, DateTime start, DateTime end)
        {
            var entity = entities.Events.Find(id);
            entity.Start = start;
            entity.End = end;

            entities.SaveChanges();
        }
    }
}