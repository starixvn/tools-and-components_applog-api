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
        public List<ScheduleModel> Schedules
        {
            get
            {
                return Entities.Schedules.Select(x => new ScheduleModel
                {
                    Id = x.Id,
                    StartDate = x.StartDate.Value,
                    EndDate = x.EndDate.Value,
                    Name = x.Name,
                    Repeat = (Repeat)x.Repeat,
                    GMT = x.GMT.Value
                }).ToList();
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