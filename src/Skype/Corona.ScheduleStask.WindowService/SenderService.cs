using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Core;
using Corona.Models;


namespace Corona.ScheduleTask.WindowService
{
    public class SenderService : BaseService
    {
        private readonly UnderMaintenanceService underMaintenanceService;

        public SenderService()
        {
            underMaintenanceService = new UnderMaintenanceService();
        }

        public void Run()
        {
            var functions = GetFunctions();
            var events = GetEvents();

            functions.ForEach(f =>
            {
                var model = events.First(x => x.Functions.Id == f.Id);

                if (f.Key == FunctionKeyModel.UnderMaintenace)
                {
                    underMaintenanceService.ExecuteUMTask(model);
                }
                else
                {
                    ExecuteTask(model);
                }               
            });
        }        

        private void ExecuteTask(EventModel model)
        {
            var currentTime = DateTime.UtcNow.AddHours(model.Schedule.GMT);

            if (Utilities.DatetimeComparer(currentTime, model.Schedule.StartDate) >= 0 && Utilities.DatetimeComparer(currentTime, model.Schedule.EndDate) <= 0)
            {
                var startDate = model.Schedule.StartDate;
                var endDate = model.Schedule.EndDate;

                if (model.Schedule.Repeat == Repeat.Daily)
                {
                    startDate = startDate.AddDays(1);
                    endDate = endDate.AddDays(1);
                }
                else if (model.Schedule.Repeat == Repeat.Weekly)
                {
                    startDate = startDate.AddMonths(1);
                    endDate = endDate.AddMonths(1);
                }
                else
                {
                    startDate = startDate.AddYears(1);
                    endDate = endDate.AddYears(1);
                }

                Send(model, startDate, endDate, false);

                if (model.Child.Any())
                {
                    model.Child.ToList().ForEach(c =>
                    {
                        ExecuteTask(c);
                    });
                }
            }
        }                
    }
}
