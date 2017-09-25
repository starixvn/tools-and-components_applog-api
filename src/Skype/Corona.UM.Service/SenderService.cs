using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Model;
using Corona.Models;
using Newtonsoft.Json;
using ONEbook.UM.Models;
using ONEbook.UM.Services.Impl;
using RestSharp;

namespace Corona.UM.Service
{
    static class SenderService
    {
        public static IList<ConversationModel> Conversations()
        {
            RestRequest request = new RestRequest(Method.GET);
            var client = new RestClient(AppSettingService.BotAPIBaseUrl);
            request.Resource = AppSettingService.BotAPIResource + "/" + "Conversations/";

            return client.Execute<List<ConversationModel>>(request).Data;
        }

        public static IList<EventModel> Events()
        {
            RestRequest request = new RestRequest(Method.GET);
            var client = new RestClient(AppSettingService.BotAPIBaseUrl);
            request.Resource = AppSettingService.BotAPIResource + "/" + "UMEvents/";

            return client.Execute<List<EventModel>>(request).Data;
        }

        public static bool UpdateEvent(EventModel model)
        {
            var client = new RestClient(AppSettingService.BotAPIBaseUrl);
            var request = new RestRequest(Method.POST);
            request.Resource = AppSettingService.BotAPIResource + "/" + "UpdateEvent/";

            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("model", model, ParameterType.QueryString);
            request.JsonSerializer.ContentType = "application/json; charset=utf-8";

            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public static bool SendMessage(RequestModel parameters, string actionName)
        {
            var baseUrl = AppSettingService.BotAPIBaseUrl;
            var client = new RestClient(baseUrl);

            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.Resource = AppSettingService.BotAPIResource + "/" + actionName + "/";
            request.AddParameter("application/json", JsonConvert.SerializeObject(parameters), ParameterType.RequestBody);

            var response = client.Execute<RequestModel>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public static void Send()
        {
            var events = Events();
            var um = UMTime();
            var isUm = um.IsUnderMaintenanceTime;
            var umFrom = new DateTime();
            var umTo = new DateTime();
            var umConfig = ReadUMConfig();
            var currentDate = DateTime.UtcNow.AddHours(AppSettingService.TimeZone);

            currentDate = Convert.ToDateTime(currentDate.ToString("MM/dd/yyyy HH:mm"));

            if (umConfig.Any())
            {
                umFrom = Convert.ToDateTime(umConfig["from"].ToString("MM/dd/yyyy HH:mm"));
                umTo = Convert.ToDateTime(umConfig["to"].ToString("MM/dd/yyyy HH:mm"));
            }
            else
            {
                umFrom = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
                umTo = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
            }

            if (events != null)
            {
                var notifyEvent = events.First(x => x.Start.HasValue && x.End.HasValue);
                if (!notifyEvent.IsSent)
                {
                    if (umFrom != notifyEvent.Start && umTo != notifyEvent.End)
                    {
                        UMNotification(events, umFrom, umTo);
                    }
                }

                if (!isUm)
                {                    
                    var startUmEvents = events.First(x => !x.Start.HasValue);

                    if (startUmEvents.IsSent)
                    {
                        var endUmEvents = events.First(x => !x.End.HasValue);

                        UnderMaintenanceStatus(new EventModel
                        {
                            End = umTo,
                            Start = umFrom,
                            Id = endUmEvents.Id,
                            IsSent = endUmEvents.IsSent,
                            IsUM = endUmEvents.IsUM,
                            Message = endUmEvents.Message,
                            Repeat = endUmEvents.Repeat
                        });
                    }
                }
                else
                {
                    var startUmEvents = events.First(x => !x.Start.HasValue);

                    if (startUmEvents.Start <= currentDate)
                    {
                        UnderMaintenanceStatus(new EventModel
                        {
                            End = umTo,
                            Start = umFrom,
                            Id = startUmEvents.Id,
                            IsSent = startUmEvents.IsSent,
                            IsUM = startUmEvents.IsUM,
                            Message = startUmEvents.Message,
                            Repeat = startUmEvents.Repeat
                        });
                    }
                }
            }
        }

        private static Dictionary<string, DateTime> ReadUMConfig()
        {
            var umPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\UM.txt");
            var data = File.ReadAllLines(umPath);

            if (data.Any())
            {
                var result = new Dictionary<string, DateTime>();
                result.Add("from", Convert.ToDateTime(data[0]));
                result.Add("to", Convert.ToDateTime(data[1]));

                return result;
            }

            return new Dictionary<string, DateTime>();
        }

        private static UnderMaintenanceTime UMTime()
        {
            var maintenanceService = new MaintenanceService();

            return maintenanceService.GetUnderMaintenanceTime();
        }

        private static void UMNotification(IList<EventModel> events, DateTime from, DateTime to)
        {
            if (events != null)
            {
                var model = events.First(x => x.Start.HasValue && x.End.HasValue && x.IsUM);

                if (model.Start.HasValue && model.End.HasValue)
                {
                    var oldStart = Convert.ToDateTime(model.Start.Value.ToString("MM/dd/yyyy HH:mm"));
                    var oldEnd = Convert.ToDateTime(model.End.Value.ToString("MM/dd/yyyy HH:mm"));
                    var currentUMStart = Convert.ToDateTime(from.ToString("MM/dd/yyyy HH:mm"));
                    var currentUMEnd = Convert.ToDateTime(to.ToString("MM/dd/yyyy HH:mm"));

                    if (!model.IsSent && oldStart != currentUMStart && oldEnd != currentUMEnd)
                    {
                        var message = string.Format(model.Message, from.ToString("HH:mm"), to.ToString("HH:mm"), from.ToString("MM/dd/yyyy"));

                        var param = new RequestModel
                        {
                            IsNotification = true,
                            Events = new EventModel
                            {
                                Start = from,
                                End = to,
                                Id = model.Id,
                                IsSent = model.IsSent,
                                IsUM = model.IsUM,
                                Message = message,
                                Repeat = model.Repeat
                            }
                        };

                        SendMessage(param, "UMMessage");
                    }
                }
            }
        }

        private static string ExtendUMNofitication(DateTime oldFrom, DateTime oldTo, DateTime newFrom, DateTime newTo, string message)
        {
            return string.Empty;
        }

        private static void UnderMaintenanceStatus(EventModel model)
        {
            if (!model.IsSent)
            {
                var from = model.Start.Value.ToString("HH:mm");
                var to = model.End.Value.ToString("HH:mm");
                var umDate = model.Start.Value.ToString("MM/dd/yyyy");

                var message = string.Format(model.Message, from, to, umDate);

                var param = new RequestModel
                {
                    IsNotification = false,
                    Events = new EventModel
                    {
                        Start = model.Start,
                        End = model.End,
                        Id = model.Id,
                        IsSent = true,
                        IsUM = model.IsUM,
                        Message = message,
                        Repeat = model.Repeat
                    }
                };

                SendMessage(param, "UMMessage");
            }
        }
    }
}
