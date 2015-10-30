namespace AppLogApi.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using AppLogApi.Criteria;
    using AppLogApi.Models.AppLog;
    using AppLogApi.Services.Impls;
    using AppLogApi.Services.Interfaces;

    public class LogManagerController : ApiController
    {
        private readonly IAppLogService _appLogService;

        public LogManagerController()
            : this(null)
        {
        }

        public LogManagerController(IAppLogService appLogService)
        {
            _appLogService = appLogService ?? new AppLogService();
        }

        [HttpGet]
        public IList<AppLogViewModel> GetAppLog([FromUri]GetAppLogCriteria criteria)
        {
            var appLogs = new List<AppLogViewModel>();

            if (criteria != null)
            {
                appLogs = _appLogService.GetAppLog(criteria).ToList();

                if (appLogs.Count > 0)
                {
                    string groupChatXmlPath = HttpContext.Current.Server.MapPath("~/App_Data/GroupChat.xml");

                    appLogs = _appLogService.MappGroupChatAndGenerateUrlAndReferrer(groupChatXmlPath, appLogs).ToList();
                }
            }

            return appLogs;
        }
    }
}
