using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Fanex.Logging;
using Letstalk.Bots.DatabaseLogSender.Models;
using ONEbook.NotificationService;

namespace Letstalk.Bots.DatabaseLogSender.Services
{
    public static class MessageSenderService
    {
        public static async Task LetstalkSender(string targetName, Dictionary<string, string> vars)
        {
            var serverUrls = AppSetting.LetstalkAPI.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Notification.Initialize(serverUrls, AppSetting.LetstalkKey);

            Notification.SendAsync(targetName, vars);
        }

        public static async Task SkypeSender(MessageModel messageModel)
        {
            string apiBasePath = AppSetting.CoronaService;

            var customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync(apiBasePath + "api/SendMessage", messageModel);

            if (!response.IsSuccessStatusCode)
            {
                LogService.Error(string.Format("Failed to call the API. HTTP Status: {0}, Reason {1}, Response header {2}", response.StatusCode, response.ReasonPhrase, response.Headers));
            }
        }
    }
}
