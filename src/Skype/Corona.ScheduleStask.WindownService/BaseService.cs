using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Model;
using Newtonsoft.Json;
using RestSharp;

namespace Corona.ScheduleTask.WindownService
{
    public class BaseService
    {
        public static string BotAPIBaseUrl => ConfigurationManager.AppSettings["BotApi.BaseUrl"];
        public static string BotAPIData => ConfigurationManager.AppSettings["BotApi.Data"];
        public static string BotAPIResume => ConfigurationManager.AppSettings["BotApi.Resume"];
        public static int TaskInterval => Convert.ToInt32(ConfigurationManager.AppSettings["TaskInterval"]);

        public static IList<T> GetData<T>(string actionName)
        {
            var request = new RestRequest(Method.GET);
            var client = new RestClient(BotAPIBaseUrl);
            request.Resource = string.Format(BotAPIData + "/{0}/", actionName);

            return client.Execute<List<T>>(request).Data;
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
    }
}
