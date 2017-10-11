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
        private List<EventModel> GetEvent() => GetData<EventModel>("Events").Where(x => !x.IsUM).ToList();

        public void Run()
        {
            var events = GetEvent();
            var newEvents = new List<EventModel>();

            events.ForEach(x =>
            {
                var currentTime = DateTime.UtcNow.AddHours(x.GMT);                               

                if (Utilities.DatetimeComparer(currentTime, x.Start.Value) >= 0 && Utilities.DatetimeComparer(currentTime, x.End.Value) <=0)
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
                SendMessage(new Model.RequestModel
                {
                    Events = newEvents
                }, "SendMessage");
            }
        }
    }
}
