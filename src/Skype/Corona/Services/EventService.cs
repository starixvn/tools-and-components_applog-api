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
        void Delete(int id);

        void UpdateEvent(int id, DateTime? start, DateTime? end);

        void UpdateEvent2(int id, int scheduleId, bool isSent);
        
        IList<EventModel2> Events2(int parentId = 0);

        IList<EventModel2> Events2List();

        EventModel2 GetDirectParent(int ParentId);

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

       

        public bool Existing(int id)
        {
            return Entities.Events.Any(x => x.Id == id);
        }

        public bool Existing2(int id)
        {
            return Entities.Event2.Any(x => x.Id == id);
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