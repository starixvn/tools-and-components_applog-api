using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Corona.EntityModel;
using Corona.Models;

namespace Corona.Services
{
    public interface IEventService
    {
        int Insert(EventModel model);

        void Update(int id, EventModel model);

        void Delete(int id);

        void UpdateEvent(int id, DateTime? start, DateTime? end);

        IList<EventModel> Events { get; }

        EventModel Event(int id);

        void UpdateMessageSent(int id, bool isSent);

        bool Existing(int id);
    }
    public class EventService : QueryService, IEventService
    {
        public IList<EventModel> Events
        {
            get
            {
                return Entities.Events.Select(x => new EventModel
                {
                    Id = x.Id,
                    Start = x.Start.Value,
                    Message = x.Message,
                    Repeat = (Repeat)x.Repeat.Value,
                    End = x.End.Value,
                    IsUM = x.IsUM.Value,
                    IsSent = x.IsSent.Value
                }).ToList();
            }
        }

        public void Delete(int id)
        {
            var entity = Entities.Events.Find(id);
            Entities.Entry(entity).State = EntityState.Modified;
            Entities.Events.Remove(entity);
            Entities.SaveChanges();
        }

        public EventModel Event(int id)
        {
            return Events.First(x => x.Id == id);
        }

        public bool Existing(int id)
        {
            return Entities.Events.Any(x => x.Id == id);
        }

        public int Insert(EventModel model)
        {
            var eventModel = new EntityModel.Event
            {
                Start = model.Start,
                Message = model.Message,
                Repeat = (byte)model.Repeat,
                End = model.End,
                IsUM = model.IsUM,
                IsSent = model.IsSent
            };

            Entities.Entry(eventModel).State = EntityState.Modified;

            Entities.Events.Add(eventModel);

            Entities.SaveChanges();

            return eventModel.Id;
        }

        public void Update(int id, EventModel model)
        {
            var entity = Entities.Events.Find(id);

            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Modified;
                entity.Start = model.Start;
                entity.Message = model.Message;
                entity.Repeat = (byte)model.Repeat;
                entity.End = model.End;
                entity.IsUM = model.IsUM;
                entity.IsSent = model.IsSent;

                Entities.SaveChanges();
            }
        }

        public void UpdateEvent(int id, DateTime? start, DateTime? end)
        {
            var entity = Entities.Events.Find(id);
            Entities.Entry(entity).State = EntityState.Modified;
            entity.Start = start;
            entity.End = end;

            Entities.SaveChanges();
        }

        public void UpdateMessageSent(int id, bool isSent)
        {
            var entity = Entities.Events.Find(id);

            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Modified;
                entity.IsSent = isSent;

                Entities.SaveChanges();
            }
        }
    }
}