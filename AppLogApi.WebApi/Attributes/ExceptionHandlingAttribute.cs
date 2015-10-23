namespace AppLogApi.WebApi.Attributes
{
    using System;
    using System.Web;
    using System.Web.Http.Filters;
    using Fanex.Logging;

    /// <summary>
    /// Exception Handling Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Function to write log for API controller exception.
        /// </summary>
        /// <param name="actionExecutedContext">HttpActionExecuted Context.</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var localFlag = actionExecutedContext.Request.Properties["MS_IsLocal"] as Lazy<bool>;
            if (localFlag == null || !localFlag.Value)
            {
                var exception = actionExecutedContext.Exception;
                string message = exception.Message +
                        "\r\nSource: " + exception.Source +
                        "\r\nDomain: " + HttpContext.Current.Request.Headers["Host"] +
                        "\r\nTargetSite: " + exception.TargetSite +
                        "\r\nStackTrace: " + exception.StackTrace;

                LogService.Error(message);
            }
        }
    }
}