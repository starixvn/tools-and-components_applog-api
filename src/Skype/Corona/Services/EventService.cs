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

        void UpdateEvent2(int id, int scheduleId, bool isSent);

        IList<EventModel> Events { get; }

        IList<EventModel2> Events2(int parentId = 0);

        IList<EventModel2> Events2List();

        EventModel2 GetDirectParent(int ParentId);

        EventModel Event(int id);

        void UpdateMessageSent(int id, bool isSent);

        void UpdateMessageSent2(int id, bool isSent);

        bool Existing(int id);

        bool Existing2(int id);
    }
    public class EventService : QueryService, IEventService
    {
        readonly IFunctionService functionService;
        readonly IScheduleService scheduleService;
        public EventService()
        {
            functionService = new FunctionService();
            scheduleService = new ScheduleService();
        }

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
                    IsSent = x.IsSent.Value,
                    GMT = x.GMT.Value,
                    MessageFile = x.MessageFile
                }).ToList();
            }
        }

        public IList<EventModel2> Events2(int parentId = 0)
        {
            var eventList = new List<EventModel2>();
            var entities = Entities.Event2.Where(x => x.ParentEventId == parentId);
            foreach (var entity in entities)
            {
                eventList.Add(new EventModel2
                {
                    Id = entity.Id,                    
                    Functions = functionService.Function(entity.FunctionId.Value),                    
                    IsSent = entity.IsSent.Value,
                    Message = entity.Message,
                    MessageFile = entity.MessageFile,
                    Name = entity.Name,
                    Child = Events2(entity.Id),                   
                    ParentId = entity.ParentEventId,
                    Schedule = scheduleService.Schedule(entity.ScheduleId),
                    KeyName = entity.KeyName
                });
            }

            return eventList;
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

        public bool Existing2(int id)
        {
            return Entities.Event2.Any(x => x.Id == id);
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
                IsSent = model.IsSent,
                GMT = model.GMT,
                MessageFile = model.MessageFile
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
                entity.GMT = model.GMT;
                entity.MessageFile = model.MessageFile;

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

        public void UpdateEvent2(int id, int scheduleId , bool isSent)
        {
            var entity = Entities.Event2.Find(id);
            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Modified;
                entity.ScheduleId = scheduleId;
                entity.IsSent = isSent;

                Entities.SaveChanges();
            }
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

        public void UpdateMessageSent2(int id, bool isSent)
        {
            var entity = Entities.Event2.Find(id);

            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Modified;
                entity.IsSent = isSent;

                Entities.SaveChanges();
            }
        }

        public IList<EventModel2> Events2List()
        {
            var eventList = new List<EventModel2>();            
            foreach (var entity in Entities.Event2)
            {
                eventList.Add(new EventModel2
                {
                    Id = entity.Id,                   
                    Functions = functionService.Function(entity.FunctionId.Value),                   
                    IsSent = entity.IsSent.Value,
                    Message = entity.Message,
                    MessageFile = entity.MessageFile,
                    Name = entity.Name,
                    Child = null,                   
                    ParentId = entity.ParentEventId,
                    Schedule = scheduleService.Schedule(entity.ScheduleId),
                    KeyName = entity.KeyName
                });
            }

            return eventList;
        }

        public EventModel2 GetDirectParent(int ParentId)
        {
            return Events2List().First(x => x.Id == ParentId);                        
        }      
    }
}