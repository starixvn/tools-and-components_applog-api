using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Corona.Models;

namespace Corona.Services
{
    public interface IScheduleService
    {
        List<ScheduleModel> Schedules { get; }
        ScheduleModel Schedule(int id);

        void UpdateSchedule(int id, DateTime startDate, DateTime endDate);
    }
    public class ScheduleService : QueryService, IScheduleService
    {

        private ScheduleModel Transform(EntityModel.Schedule model)
        {
            return new ScheduleModel
            {
                Id = model.Id,
                StartDate = model.StartDate.Value,
                EndDate = model.EndDate.Value,
                Name = model.Name,
                Repeat = (Repeat)model.Repeat,
                GMT = model.GMT.Value
            };
        }

        public List<ScheduleModel> Schedules
        {
            get
            {
                var schedules = new List<ScheduleModel>();

                Entities.Schedules.ToList().ForEach(x => schedules.Add(Transform(x)));

                return schedules;
            }
        }

        public ScheduleModel Schedule(int id)
        {
            return Schedules.First(x => x.Id == id);
        }

        public void UpdateSchedule(int id, DateTime startDate, DateTime endDate)
        {
            var entity = Entities.Schedules.Find(id);
            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Modified;
                entity.StartDate = startDate;
                entity.EndDate = endDate;

                Entities.SaveChanges();
            }
        }
    }
}