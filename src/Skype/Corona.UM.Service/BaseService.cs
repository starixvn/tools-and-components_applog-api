using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Models;
using Newtonsoft.Json;
using RestSharp;
using Sunplus.UMControllerLibrary;

namespace Corona.UM.Service
{
    public class BaseService
    {
        public static bool SendMessage(UMRequestModel parameters, string actionName)
        {
            var client = new RestClient(AppSettingService.BotAPIBaseUrl);
            var request = new RestRequest(Method.POST);

            request.RequestFormat = DataFormat.Json;
            request.Resource = AppSettingService.BotAPIResume + "/" + actionName + "/";
            request.AddParameter("application/json", JsonConvert.SerializeObject(parameters), ParameterType.RequestBody);

            var response = client.Execute<RequestModel>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public static UnderMaintenanceModel UnderMaintenance()
        {
            var umController = new UMBusiness();
            var umFrom = string.Empty;
            var umTo = string.Empty;
            var currentDate = DateTime.UtcNow.AddHours(AppSettingService.TimeZone);

            var isUM = !AppSettingService.IsLocal && umController.isUnderMaintainence(Sites.Agency, AppSettingService.UMServiceUrl, ref umFrom, ref umTo);

            return new UnderMaintenanceModel
            {
                From = Convert.ToDateTime(currentDate.ToString("MM/dd/yyyy") + " " + umFrom),
                To = Convert.ToDateTime(currentDate.ToString("MM/dd/yyyy") + " " + umTo),
                IsUM = isUM ? 1 : 0
            };
        }

        public static IList<T> GetData<T>(string actionName)
        {
            RestRequest request = new RestRequest(Method.GET);
            var client = new RestClient(AppSettingService.BotAPIBaseUrl);
            request.Resource = string.Format(AppSettingService.BotAPIData + "/{0}/", actionName);

            return client.Execute<List<T>>(request).Data;
        }

    }
}
