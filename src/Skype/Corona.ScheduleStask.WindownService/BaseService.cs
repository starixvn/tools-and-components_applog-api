using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Core;
using Corona.Models;
using Newtonsoft.Json;
using RestSharp;
using Sunplus.UMControllerLibrary;

namespace Corona.ScheduleTask.WindownService
{
    public class BaseService
    {
        public static string BotAPIBaseUrl => ConfigurationManager.AppSettings["BotApi.BaseUrl"];
        public static string BotAPIData => ConfigurationManager.AppSettings["BotApi.Data"];
        public static string BotAPIResume => ConfigurationManager.AppSettings["BotApi.Resume"];
        public static int TaskInterval => Convert.ToInt32(ConfigurationManager.AppSettings["TaskInterval"]);

        public static IList<T> GetDataList<T>(string actionName)
        {
            var request = new RestRequest(Method.GET);
            var client = new RestClient(BotAPIBaseUrl);
            request.Resource = string.Format(BotAPIData + "/{0}/", actionName);
            var result = client.Execute<List<T>>(request);

            return JsonConvert.DeserializeObject<List<T>>(result.Content);
        }

        public static T GetData<T>(string actionName, int parentId)
        {
            var request = new RestRequest(Method.GET);
            var client = new RestClient(BotAPIBaseUrl);
            request.Resource = string.Format(BotAPIData + "/{0}/", actionName);
            request.AddParameter("ParentId", parentId);

            var result = client.Execute(request);

            return JsonConvert.DeserializeObject<T>(result.Content);
        }

        public static bool SendMessage(RequestModel parameters, string actionName)
        {
            var client = new RestClient(BotAPIBaseUrl);
            var request = new RestRequest(Method.POST);

            request.RequestFormat = DataFormat.Json;
            request.Resource = string.Format(BotAPIResume + "/{0}/", actionName);
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

        public static UnderMaintenanceModel UnderMaintenance()
        {
            var umController = new UMBusiness();
            var umFrom = string.Empty;
            var umTo = string.Empty;
            var currentDate = DateTime.UtcNow.AddHours(AppSetting.TimeZone);

            var isUM = !AppSetting.IsLocal && umController.isUnderMaintainence(Sites.Agency, AppSetting.UMServiceUrl, ref umFrom, ref umTo);

            return new UnderMaintenanceModel
            {
                From = Convert.ToDateTime(currentDate.ToString("MM/dd/yyyy") + " " + umFrom),
                To = Convert.ToDateTime(currentDate.ToString("MM/dd/yyyy") + " " + umTo),
                IsUM = isUM ? 1 : 0
            };
        }
    }
}
