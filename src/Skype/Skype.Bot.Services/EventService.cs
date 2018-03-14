using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Corona.Models;
using Skype.Bot.Services;

namespace Skype.Bot.Services
{
    public interface IEventService
    {
        void Delete(int id);
        
        void UpdateEvent(int id, int scheduleId, bool isSent);
        
        IList<EventModel> Events(int parentId = 0);

        IList<EventModel> EventsList();

        EventModel GetDirectParent(int ParentId);

        void UpdateMessageSent(int id, bool isSent);                

        bool Existing(int id);
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

        public IList<EventModel> Events(int parentId = 0)
        {
            var eventList = new List<EventModel>();
            var entities = Entities.Events.Where(x => x.ParentEventId == parentId);
            foreach (var entity in entities)
            {
                eventList.Add(new EventModel
                {
                    Id = entity.Id,                    
                    Functions = functionService.Function(entity.FunctionId.Value),                    
                    IsSent = entity.IsSent.Value,
                    Message = entity.Message,
                    MessageFile = entity.MessageFile,
                    Name = entity.Name,
                    Child = Events(entity.Id),                   
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

        public void UpdateEvent(int id, int scheduleId , bool isSent)
        {
            var entity = Entities.Events.Find(id);
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

        public IList<EventModel> EventsList()
        {
            var eventList = new List<EventModel>();            
            foreach (var entity in Entities.Events)
            {
                eventList.Add(new EventModel
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

        public EventModel GetDirectParent(int ParentId)
        {
            return EventsList().First(x => x.Id == ParentId);                        
        }        
    }
}