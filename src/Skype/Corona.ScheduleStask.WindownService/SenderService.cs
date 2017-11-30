using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Core;
using Corona.Models;


namespace Corona.ScheduleTask.WindownService
{
    public class SenderService : BaseService
    {
        private readonly UnderMaintenanceService underMaintenanceService;

        public SenderService()
        {
            underMaintenanceService = new UnderMaintenanceService();
        }

        public void Run2()
        {
            var functions = GetFunctions();
            var events = GetEvents2();

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

        private void ExecuteTask(EventModel2 model)
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

        public void Run()
        {
            var events = GetEvent();
            var newEvents = new List<EventModel>();

            events.ForEach(x =>
            {
                var currentTime = DateTime.UtcNow.AddHours(x.GMT);

                if (Utilities.DatetimeComparer(currentTime, x.Start.Value) >= 0 && Utilities.DatetimeComparer(currentTime, x.End.Value) <= 0)
                {
                    var startDate = x.Start;
                    var endDate = x.End;

                    if (x.Repeat == Repeat.Daily)
                    {
                        startDate = startDate.Value.AddDays(1);
                        endDate = endDate.Value.AddDays(1);
                    }
                    else if (x.Repeat == Repeat.Weekly)
                    {
                        startDate = startDate.Value.AddMonths(1);
                        endDate = endDate.Value.AddMonths(1);
                    }
                    else
                    {
                        startDate = startDate.Value.AddYears(1);
                        endDate = endDate.Value.AddYears(1);
                    }

                    var message = x.Message;
                    if (!string.IsNullOrWhiteSpace(x.MessageFile))
                    {
                        var messages = Utilities.ReadMessage(x.MessageFile);
                        message = Utilities.RandomMessage(messages);
                    }

                    newEvents.Add(new EventModel
                    {
                        Id = x.Id,
                        Start = startDate,
                        End = endDate,
                        GMT = x.GMT,
                        IsSent = x.IsSent,
                        IsUM = x.IsUM,
                        Message = message,
                        Repeat = x.Repeat
                    });
                }
            });

            if (newEvents.Any())
            {
                SendMessage(new RequestModel
                {
                    Events = newEvents
                }, "SendMessage");
            }
        }
    }
}
