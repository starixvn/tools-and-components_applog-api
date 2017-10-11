using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Core;
using Corona.Model;
using Corona.Models;
using Corona.UM.Service;
using Newtonsoft.Json;
using RestSharp;
using Sunplus.UMControllerLibrary;

namespace Corona.UM.Service
{
    public class UMSenderService : BaseService
    {
        public static IList<ConversationModel> Conversations()
        {
            return GetData<ConversationModel>("Conversations");
        }

        public static IList<EventModel> Events()
        {       
            return GetData<EventModel>("UMEvents");
        }

        public static void RunService()
        {
            var isUm = false;
            var umConfigFrom = DateTime.Now;
            var umConfigTo = DateTime.Now;

            var umConfig = ReadUMConfig();
            var umService = UnderMaintenance();

            if (umConfig.Any())
            {
                umConfigFrom = Utilities.DatetimeConverter(Convert.ToDateTime(umConfig["from"]));                    
                umConfigTo = Utilities.DatetimeConverter(Convert.ToDateTime(umConfig["to"]));                

                isUm = umConfig.Count > 2 ? Convert.ToBoolean(umConfig["isum"]) : umService.IsUM;
            }
            else
            {
                umConfigFrom = Utilities.DatetimeConverter(umService.From);
                umConfigTo = Utilities.DatetimeConverter(umService.To);

                isUm = umService.IsUM;
            }

            var events = Events();

            if (events != null)
            {               
                RunUMNotification(events, umConfigFrom, umConfigTo);

                if (!isUm)
                {
                    RunUMStoped(isUm, events, umConfigFrom, umConfigTo);
                }
                else
                {
                    RunUMStartAndExtend(isUm, events, umConfigFrom, umConfigTo);
                }
            }
        }

        private static void RunUMStartAndExtend(bool isUm, IList<EventModel> events, DateTime umFrom, DateTime umTo)
        {
            var startUmEvents = events.First(x => x.Start.HasValue && !x.End.HasValue);

            if (!startUmEvents.IsSent)
            {
                UnderMaintenanceStatus(new EventModel
                {
                    End = umTo,
                    Start = umFrom,
                    Id = startUmEvents.Id,
                    IsSent = startUmEvents.IsSent,
                    IsUM = startUmEvents.IsUM,
                    Message = startUmEvents.Message,
                    Repeat = startUmEvents.Repeat,
                    MessageFile = startUmEvents.MessageFile
                }, isUm);
            }
            else
            {
                ExtendUMNofitication(events, umTo, isUm);
            }
        }

        private static void RunUMStoped(bool isUm, IList<EventModel> events, DateTime umFrom, DateTime umTo)
        {
            var startUmEvents = events.First(x => x.Start.HasValue && !x.End.HasValue);

            if (startUmEvents.IsSent)
            {
                var endUmEvents = events.First(x => x.End.HasValue && !x.Start.HasValue);

                UnderMaintenanceStatus(new EventModel
                {
                    End = umTo,
                    Start = umFrom,
                    Id = endUmEvents.Id,
                    IsSent = endUmEvents.IsSent,
                    IsUM = endUmEvents.IsUM,
                    Message = endUmEvents.Message,
                    Repeat = endUmEvents.Repeat,
                    MessageFile = endUmEvents.MessageFile
                }, isUm);
            }
        }

        private static void RunUMNotification(IList<EventModel> events, DateTime umFrom, DateTime umTo)
        {
            var notifyEvent = events.First(x => x.Start.HasValue && x.End.HasValue);

            if (!notifyEvent.IsSent && umFrom != notifyEvent.Start && umTo != notifyEvent.End)
            {
                UMNotification(events, umFrom, umTo);
            }
        }

        private static Dictionary<string, object> ReadUMConfig()
        {
            var umPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\UMConfig.txt");
            var data = File.ReadAllLines(umPath);

            if (data.Any())
            {
                var result = new Dictionary<string, object>();
                result.Add("from", data[0]);
                result.Add("to", data[1]);

                if (data.Count() > 2)
                {
                    result.Add("isum", data[2] == "1");
                }

                return result;
            }

            return new Dictionary<string, object>();
        }        

        private static void UMNotification(IList<EventModel> events, DateTime from, DateTime to)
        {
            if (events != null)
            {
                var model = events.First(x => x.Start.HasValue && x.End.HasValue && x.IsUM);

                if (model.Start.HasValue && model.End.HasValue)
                {
                    var oldStart = Utilities.DatetimeConverter(model.Start.Value);                       
                    var oldEnd = Utilities.DatetimeConverter(model.End.Value);
                    var currentUMStart = Utilities.DatetimeConverter(from);
                    var currentUMEnd = Utilities.DatetimeConverter(to);

                    if (!model.IsSent && oldStart != currentUMStart && oldEnd != currentUMEnd)
                    {
                        var messages = Utilities.ReadMessage(model.MessageFile);                                                    
                        var randomMessage = messages.Any() ? Utilities.RandomMessage(messages) : model.Message;
                        var message = string.Format(randomMessage, from.ToString("HH:mm"), to.ToString("HH:mm"), from.ToString("dddd, MM/dd/yyyy"));

                        var param = new UMRequestModel
                        {
                            IsNotification = true,
                            IsUM = false,
                            IsExtend = false,
                            Events = new EventModel
                            {
                                Start = from,
                                End = to,
                                Id = model.Id,
                                IsSent = model.IsSent,
                                IsUM = model.IsUM,
                                Message = message,
                                Repeat = model.Repeat,
                                MessageFile = model.MessageFile
                            }
                        };

                        SendMessage(param, "UMMessage");
                    }
                }
            }
        }

        private static void ExtendUMNofitication(IList<EventModel> events, DateTime umTo, bool isUM = false)
        {
            if (isUM)
            {
                var endUM = events.First(x => !x.Start.HasValue && x.End.HasValue);
                var extendUM = events.First(x => !x.Start.HasValue && !x.End.HasValue);

                if (!extendUM.IsSent && endUM.End != umTo)
                {
                    var message = string.Format(extendUM.Message, umTo.ToString("HH:mm"));

                    var param = new UMRequestModel
                    {
                        IsNotification = false,
                        IsUM = isUM,
                        IsExtend = true,
                        Events = new EventModel
                        {
                            Start = endUM.Start,
                            End = umTo,
                            Id = extendUM.Id,
                            IsSent = true,
                            IsUM = extendUM.IsUM,
                            Message = message,
                            Repeat = extendUM.Repeat,
                            MessageFile = extendUM.MessageFile
                        }
                    };

                    SendMessage(param, "UMMessage");
                }
            }
        }

        private static void UnderMaintenanceStatus(EventModel model, bool isUM = false)
        {
            if (!model.IsSent)
            {
                var from = model.Start.Value.ToString("HH:mm");
                var to = model.End.Value.ToString("HH:mm");
                var umDate = model.Start.Value.ToString("MMMM/dd/yyyy");
                var messages = Utilities.ReadMessage(model.MessageFile);
                var randomMessage = messages.Any() ? Utilities.RandomMessage(messages) : model.Message;
                var message = string.Format(randomMessage, from, to, umDate);
                var param = new UMRequestModel
                {
                    IsNotification = false,
                    IsUM = isUM,
                    Events = new EventModel
                    {
                        Start = model.Start,
                        End = model.End,
                        Id = model.Id,
                        IsSent = true,
                        IsUM = model.IsUM,
                        Message = message,
                        Repeat = model.Repeat,
                        MessageFile = model.MessageFile
                    }
                };

                SendMessage(param, "UMMessage");
            }
        }
    }
}
