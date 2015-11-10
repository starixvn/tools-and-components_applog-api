namespace AppLogApi.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using AppLogApi.Criteria;
    using AppLogApi.Models.AppLog;
    using AppLogApi.Services.Impls;
    using AppLogApi.Services.Interfaces;
    using Sunplus.UMControllerLibrary;

    public class LogManagerController : ApiController
    {
        private readonly IAppLogService _appLogService;
        private UMBusiness _umService;

        public LogManagerController()
            : this(null, null)
        {
        }

        public LogManagerController(IAppLogService appLogService, UMBusiness umService)
        {
            _appLogService = appLogService ?? new AppLogService();
            _umService = umService ?? new UMBusiness();
        }

        [HttpGet]
        public IList<AppLogViewModel> GetAppLog([FromUri]GetAppLogCriteria criteria)
        {
            var appLogs = new List<AppLogViewModel>();
            bool isUnderMaintenance = _umService.isUnderMaintainence(Sites.Agency, ConfigurationManager.AppSettings["UnderMaintenanceUrl"]);

            if (!isUnderMaintenance && criteria != null)
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
