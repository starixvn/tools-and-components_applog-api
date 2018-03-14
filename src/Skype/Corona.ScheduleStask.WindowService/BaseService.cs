using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Corona.Core;
using Corona.Models;
using Newtonsoft.Json;
using RestSharp;
using Sunplus.UMControllerLibrary;

namespace Corona.ScheduleTask.WindowService
{
    public class BaseService
    {
        public static List<T> GetData<T>(string actionName, Dictionary<string, object> parameters = null)
        {
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/xml");
            var basePath = new Uri(AppSetting.BotAPIBaseUrl);
            var client = new RestClient(basePath);
            request.Resource = string.Format(AppSetting.BotAPIData + "/{0}/", actionName);

            if (parameters != null && parameters.Any())
            {
                foreach (var param in parameters)
                {
                    request.AddParameter(param.Key, param.Value);
                }
            }

            var result = client.Execute(request);

            if (!string.IsNullOrWhiteSpace(result.Content))
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<T>>(result.Content);
                }
                catch
                {
                    Fanex.Logging.LogService.Error("Result.Content=" + result.Content);
                }
            }

            return null;
        }


        public static bool SendMessage(RequestModel parameters, string actionName)
        {
            var basePath = new Uri(AppSetting.BotAPIBaseUrl);
            var client = new RestClient(basePath);

            var request = new RestRequest(Method.POST);

            request.RequestFormat = DataFormat.Json;
            request.Resource = string.Format(AppSetting.BotAPIResume + "/{0}/", actionName);
            request.AddParameter("application/json", JsonConvert.SerializeObject(parameters), ParameterType.RequestBody);

            var response = client.Execute<RequestModel>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public static UnderMaintenanceModel UMConfig()
        {
            var umPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\UMConfig.txt");
            var data = File.ReadAllLines(umPath);

            if (data.Any())
            {
                return new UnderMaintenanceModel
                {
                    IsUM = Convert.ToInt16(data[2]),
                    From = Convert.ToDateTime(data[0]),
                    To = Convert.ToDateTime(data[1])
                };
            }

            return new UnderMaintenanceModel();
        }

        public static UnderMaintenanceModel UnderMaintenance(int timeZone)
        {
            var umController = new UMBusiness();
            var umFrom = string.Empty;
            var umTo = string.Empty;
            var currentDate = DateTime.UtcNow.AddHours(timeZone);

            var isUM = !AppSetting.IsLocal && umController.isUnderMaintainence(Sunplus.UMControllerLibrary.Sites.Agency, AppSetting.UMServiceUrl, ref umFrom, ref umTo);

            return new UnderMaintenanceModel
            {
                From = Convert.ToDateTime(currentDate.ToString("MM/dd/yyyy") + " " + umFrom),
                To = Convert.ToDateTime(currentDate.ToString("MM/dd/yyyy") + " " + umTo),
                IsUM = isUM ? 1 : 0
            };
        }

        public static List<EventModel2> GetEvents2() => GetData<EventModel2>("Events2");

        public static List<EventModel> GetEvents() => GetData<EventModel>("Events");

        public static List<ScheduleModel> GetSchedules() => GetData<ScheduleModel>("Schedules");

        public static List<FunctionModel> GetFunctions() => GetData<FunctionModel>("Functions");

        public void Send(EventModel model, DateTime startDate, DateTime endDate, bool isSent = true, bool isUpdateData = true)
        {
            var randomMessage = model.Message;

            if (!string.IsNullOrWhiteSpace(model.MessageFile))
            {
                var messages = Utilities.ReadMessage(model.MessageFile);
                randomMessage = messages.Any() ? Utilities.RandomMessage(messages) : model.Message;
            }

            SendMessage(new RequestModel
            {
                Event = new EventModel
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
                    Message = string.Format(randomMessage, startDate.ToString("HH:mm"), endDate.ToString("HH:mm"), endDate.ToString("dddd, MM/dd/yyyy"))
                },
                IsUpdateData = isUpdateData
            }, "SendMessage");
        }                    
    }
}
