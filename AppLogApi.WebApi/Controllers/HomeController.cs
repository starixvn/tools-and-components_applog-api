namespace AppLogApi.WebApi.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using AppLogApi.Criteria;
    using AppLogApi.Services.Impls;
    using AppLogApi.Services.Interfaces;

    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public void Test()
        {
            IAppLogService _appLogService = new AppLogService();
            GetAppLogCriteria criteria = new GetAppLogCriteria()
            {
                ToDate = new DateTime(2015, 10, 22, 18, 0, 0),
                FromDate = new DateTime(2015, 10, 22, 16, 0, 0),
                ClientTimeZone = 7,
                IsProduction = true,
                Severity = "Error, Critical"
            };

            var appLogs = _appLogService.GetAppLog(criteria).ToList();

            if (appLogs.Count > 0)
            {
                string groupChatXmlPath = Server.MapPath("~/App_Data/GroupChat.xml");

                appLogs = _appLogService.MappGroupChat(groupChatXmlPath, appLogs).ToList();
            }
        }
    }
}
