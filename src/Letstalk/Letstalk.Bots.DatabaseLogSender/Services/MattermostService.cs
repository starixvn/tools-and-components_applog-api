using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Fanex.Logging;

namespace Letstalk.Bots.DatabaseLogSender.Services
{
    public class MattermostService
    {
        private readonly HttpClient _httpClient;
        private readonly string _mattermostUrl;
        private readonly string _botToken;

        public MattermostService(string mattermostUrl, string botToken)
        {
            _mattermostUrl = mattermostUrl.TrimEnd('/');
            _botToken = botToken;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _botToken);
        }

        public async Task<bool> SendMessageAsync(string channelId, string message)
        {
            try
            {
                var payload = new
                {
                    channel_id = channelId,
                    message = message
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync($"{_mattermostUrl}/api/v4/posts", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogService.Error($"Error sending message to Mattermost: {ex.Message}", ex);
                return false;
            }
        }
    }
} 