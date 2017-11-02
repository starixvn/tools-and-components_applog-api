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
        private List<EventModel> GetEvent() => GetDataList<EventModel>("Events").Where(x => !x.IsUM).ToList();

        private List<EventModel2> GetEvents2() => GetDataList<EventModel2>("Events2").ToList();

        private List<ScheduleModel> GetSchedules() => GetDataList<ScheduleModel>("Schedules").ToList();

        private List<FunctionModel> GetFunctions() => GetDataList<FunctionModel>("Functions").ToList();

        public void Run2()
        {
            var functions = GetFunctions();
            var events = GetEvents2();

            functions.ForEach(f =>
            {
                var model = events.First(x => x.Functions.Id == f.Id);

                switch (f.Key)
                {
                    case FunctionKeyModel.UnderMaintenace:
                        {
                            ExecuteUMTask(model);

                            break;
                        }
                    case FunctionKeyModel.Morning:
                        {
                            ExecuteTask(model);
                            break;
                        }
                }

            });
        }

        private void Send(EventModel2 model, DateTime startDate, DateTime endDate, bool isSent)
        {
            var randomMessage = model.Message;

            if (!string.IsNullOrWhiteSpace(model.MessageFile))
            {
                var messages = Utilities.ReadMessage(model.MessageFile);
                randomMessage = messages.Any() ? Utilities.RandomMessage(messages) : model.Message;
            }

            SendMessage(new RequestModel
            {
                Event2 = new EventModel2
                {
                    Id = model.Id,
                    IsSent = isSent,
                    ParentId = model.ParentId,
                    Functions = model.Functions,
                    KeyName = model.KeyName,
                    Schedule = new ScheduleModel
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        Id = model.Schedule.Id
                    },
                    Message =string.Format(randomMessage, startDate.ToString("HH:mm"), endDate.ToString("HH:mm"), endDate.ToString("dddd, MM/dd/yyyy"))
                },
            }, "SendMessage2");
        }

        private void ExecuteUMTask(EventModel2 model)
        {

            if (model != null)
            {
                var umConfig = BaseService.UMConfig();
                var schedules = GetSchedules();

                if (model.IsSent)
                {
                    if (model.Child.Any())
                    {
                        model.Child.ToList().ForEach(c =>
                        {
                            ExecuteUMTask(c);
                        });
                    }
                }
                else
                {
                    var schedule = schedules.First(x => x.Id == model.Schedule.Id);
                    var oldStartTime = Utilities.DatetimeConverter(schedule.StartDate);
                    var oldEndTime = Utilities.DatetimeConverter(schedule.EndDate);
                    var newStarttime = Utilities.DatetimeConverter(umConfig.From);
                    var newEndTime = Utilities.DatetimeConverter(umConfig.To);
                    var isUM = umConfig.IsUM != -1 ? umConfig.IsUM == 1 : UnderMaintenance().IsUM == 1;
                   
                    switch (model.KeyName)
                    {
                        case EventKeyModel.UMNotified:
                            {
                                if (!isUM && oldStartTime != newStarttime && oldEndTime != newEndTime)
                                {
                                    Send(model, newStarttime, newEndTime, true);
                                }

                                break;
                            }
                        case EventKeyModel.UMStarted:
                            {
                                if (isUM)
                                {
                                    Send(model, oldStartTime, oldEndTime, true);
                                }

                                break;
                            }
                        case EventKeyModel.UMExtended:
                            {
                                if (isUM && oldEndTime != newEndTime)
                                {
                                    Send(model, oldStartTime, newEndTime, true);
                                }

                                break;
                            }
                        default:
                            {
                                if (!isUM)
                                {
                                    Send(model, oldStartTime, oldEndTime, true);
                                }

                                break;
                            }
                    }
                }
            }
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
