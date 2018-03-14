using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Fanex.Logging;

namespace Leffe.Filters
{
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class HMACAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        private static Dictionary<string, string> allowedApps = new Dictionary<string, string>();
        private readonly UInt64 requestMaxAgeInSeconds = 300;  //5 mins
        private readonly string authenticationScheme = "amx";

        public HMACAuthenticationAttribute()
        {
            if (allowedApps.Count == 0)
            {
                allowedApps.Add("4d53bce03ec34c0a911182d4c228ee6c", "A93reRTUJHsCuQSHR+L3GxqOJyDmQpCgps102ciuabc=");
                allowedApps.Add("4pq96pj25ymbbyp6sju29433zz9eurek", "a03xwS05HnikysrEAAvtldkrXq4W+Gln3mgXrxs3vJ0=");
            }
        }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            StringBuilder logs = new StringBuilder();

            var req = context.Request;

            try
            {
                if (req.Headers.Authorization != null && authenticationScheme.Equals(req.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    var rawAuthzHeader = req.Headers.Authorization.Parameter;

                    var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);
              
                    if (autherizationHeaderArray != null)
                    {
                        var APPId = autherizationHeaderArray[0];
                        var incomingBase64Signature = autherizationHeaderArray[1];
                        var nonce = autherizationHeaderArray[2];
                        var requestTimeStamp = autherizationHeaderArray[3];
                                                
                        var isValid = isValidRequest(req, APPId, incomingBase64Signature, nonce, requestTimeStamp);

                        if (isValid.Result)
                        {
                            var currentPrincipal = new GenericPrincipal(new GenericIdentity(APPId), null);
                            context.Principal = currentPrincipal;
                        }
                        else
                        {
                            context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                        }
                    }
                    else
                    {
                        context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                    }
                }
                else
                {
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                }
            }
            catch (Exception ex)
            {
               LogService.Error(ex.Message);
            }

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result);
            return Task.FromResult(0);
        }

        public bool AllowMultiple
        {
            get { return false; }
        }

        private static string[] GetAutherizationHeaderValues(string rawAuthzHeader)
        {
            var credArray = rawAuthzHeader.Split(':');
                        
            return credArray.Length == 4 ? credArray : null;           
        }

        private async Task<bool> isValidRequest(HttpRequestMessage req, string APPId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            try
            {
                string requestContentBase64String = "";               
                string requestUri = HttpUtility.UrlEncode(req.RequestUri.Host.ToLower());

                string requestHttpMethod = req.Method.Method;

                if (!allowedApps.ContainsKey(APPId))
                {                  
                    return false;
                }

                var sharedKey = allowedApps[APPId];

                var isReplay = isReplayRequest(nonce, requestTimeStamp);                              

                if (isReplay)
                {
                    return false;
                }

                byte[] hash = await ComputeHash(req.Content).ConfigureAwait(false);

                if (hash != null)
                {
                    requestContentBase64String = Convert.ToBase64String(hash);
                }

                string data = String.Format("{0}{1}{2}{3}{4}{5}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

                var secretKeyBytes = Convert.FromBase64String(sharedKey);

                byte[] signature = Encoding.UTF8.GetBytes(data);

                using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
                {
                    byte[] signatureBytes = hmac.ComputeHash(signature);                                        
                    return (incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal));
                }

                
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
                return false;
            }

        }

        private bool isReplayRequest(string nonce, string requestTimeStamp)
        {
            if (System.Runtime.Caching.MemoryCache.Default.Contains(nonce))
            {
                return true;
            }

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if ((serverTotalSeconds - requestTotalSeconds) > requestMaxAgeInSeconds)
            {
                return true;
            }

            System.Runtime.Caching.MemoryCache.Default.Add(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(requestMaxAgeInSeconds));

            return false;
        }

        private static async Task<byte[]> ComputeHash(HttpContent httpContent)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = null;
                var content = await httpContent.ReadAsByteArrayAsync().ConfigureAwait(false);
                if (content.Length != 0)
                {
                    hash = md5.ComputeHash(content);
                }
                return hash;
            }
        }
    }

    public class ResultWithChallenge : IHttpActionResult
    {
        private readonly string authenticationScheme = "amx";
        private readonly IHttpActionResult next;

        public ResultWithChallenge(IHttpActionResult next)
        {
            this.next = next;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await next.ExecuteAsync(cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(authenticationScheme));
            }

            return response;
        }
    }
}