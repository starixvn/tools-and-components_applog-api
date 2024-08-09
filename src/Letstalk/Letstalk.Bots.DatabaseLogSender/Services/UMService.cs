using Fanex.Logging;
using Letstalk.Bots.DatabaseLogSender.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Letstalk.Bots.DatabaseLogSender.Services
{
    public interface UMService
    {
        bool IsUM();
    }

    public class UMServiceImpl : UMService, IDisposable
    {
        private IHttpClientWrapper client;
        private readonly Uri umApiUrl;
        private const string TestSiteId = "9999999";

        public UMServiceImpl(string umApiUrl) : this(umApiUrl, null)
        {
        }

        public UMServiceImpl(string umApiUrl, IHttpClientWrapper client)
        {
            this.umApiUrl = new Uri(umApiUrl);
            this.client = client ?? new HttpClientWrapper();
        }

        public bool IsUM()
        {
            return IsUMAsync()
                    .GetAwaiter()
                    .GetResult();
        }

        private async Task<bool> IsUMAsync()
        {
            bool isUM = false;

            try
            {
                string content = await client.GetAsync(umApiUrl).ConfigureAwait(false);
                var umData = JsonConvert.DeserializeObject<Dictionary<string, UMInfoModel>>(content);
                var umInfos = umData.Where(x => x.Key != TestSiteId).ToDictionary(x => x.Key, x => x.Value);

                isUM = umInfos.Values.Any(x => x.IsUnderMaintenanceTime);
            }
            catch (Exception ex)
            {
                LogService.Error($"Cannot get UM info -> Error: {ex.InnerException?.Message ?? ex.Message}", ex);
            }

            return isUM;
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client?.Dispose();
                }

                client = null;
                disposedValue = true;
            }
        }

        ~UMServiceImpl()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
