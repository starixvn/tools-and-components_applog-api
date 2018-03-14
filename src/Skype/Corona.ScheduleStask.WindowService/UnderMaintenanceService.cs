using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Corona.Core;
using Corona.Models;
using Skype.Bot.Services;

namespace Corona.ScheduleTask.WindowService
{
    public class UnderMaintenanceService : BaseService
    {
        private IEnumerable<Site> disableAccBySites;
        private bool rebootServerExecuted = false;
        private bool pingServerExecuted = false;
        private bool disabledSQLAccountExecuted = false;
        
        public void ExecuteUMTask(EventModel model)
        {
            if (model != null)
            {               
                if (model.IsSent)
                {
                    if (model.Child.Any())
                    {
                        model.Child.ToList().ForEach(c =>
                        {
                            ExecuteUMTask(c);
                        });
                    }
                }
                else
                {
                    var schedules = GetSchedules();
                    var umConfig = BaseService.UMConfig();

                    UMProcessing(model, umConfig, schedules);
                }
            }
        }

        private void UMProcessing(EventModel model, UnderMaintenanceModel umConfig, IEnumerable<ScheduleModel> schedules)
        {
            var schedule = schedules.First(x => x.Id == model.Schedule.Id);
            var oldStartTime = Utilities.DatetimeConverter(schedule.StartDate);
            var oldEndTime = Utilities.DatetimeConverter(schedule.EndDate);
            var newStarttime = Utilities.DatetimeConverter(umConfig.From);
            var newEndTime = Utilities.DatetimeConverter(umConfig.To);
            var timeZone = schedule.GMT;
            var isUM = umConfig.IsUM != -1 ? umConfig.IsUM == 1 : UnderMaintenance(timeZone).IsUM == 1;

            switch (model.KeyName)
            {
                case EventKeyModel.UMNotified:
                    {
                        if (!isUM && oldStartTime != newStarttime && oldEndTime != newEndTime)
                        {
                            Send(model, newStarttime, newEndTime);
                        }

                        break;
                    }
                case EventKeyModel.UMDisabledSQL:
                    {
                        if (isUM && !disabledSQLAccountExecuted)
                        {
                            DisabledSQL(model, newStarttime, newEndTime);
                        }

                        break;
                    }
                case EventKeyModel.UMEnabledSQL:
                    {
                        if (isUM)
                        {
                            EnabledSQL(model, newStarttime, newEndTime);
                        }

                        break;
                    }
                case EventKeyModel.UMRebootServer:
                    {
                        if (isUM)
                        {
                            var rebootTime = newStarttime.AddMinutes(1);
                            var currentTime = DateTime.UtcNow.AddHours(timeZone);

                            if (Utilities.DatetimeComparer(rebootTime, currentTime) == 0)
                            {
                                Send(model, newStarttime, newEndTime);
                            }
                        }

                        break;
                    }
                case EventKeyModel.UMRebootServerProcessing:
                    {
                        if (isUM)
                        {
                            var rebootTime = newStarttime.AddMinutes(2);
                            var currentTime = DateTime.UtcNow.AddHours(timeZone);

                            if (Utilities.DatetimeComparer(rebootTime, currentTime) == 0 && !rebootServerExecuted)
                            {
                                RebootServerProcessing(model, newStarttime, newEndTime);
                            }
                        }

                        break;
                    }
                case EventKeyModel.UMPingServerProcessing:
                    {
                        if (isUM)
                        {
                            var rebootTime = newStarttime.AddMinutes(5);
                            var currentTime = DateTime.UtcNow.AddHours(timeZone);

                            if (Utilities.DatetimeComparer(rebootTime, currentTime) == 0 && !pingServerExecuted)
                            {
                                PingServerProcessing(model, newStarttime, newEndTime);
                            }
                        }

                        break;
                    }
                case EventKeyModel.UMStarted:
                    {
                        if (isUM)
                        {
                            Send(model, oldStartTime, oldEndTime);
                        }

                        break;
                    }
                case EventKeyModel.UMExtended:
                    {
                        if (isUM && oldEndTime != newEndTime)
                        {
                            Send(model, oldStartTime, newEndTime);
                        }

                        break;
                    }
                default:
                    {
                        if (!isUM)
                        {
                            Send(model, oldStartTime, oldEndTime);
                        }

                        break;
                    }
            }
        }

        private void PingServerProcessing(EventModel model, DateTime newStarttime, DateTime newEndTime)
        {
            pingServerExecuted = true;
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\ProductionServer.xml";
            var serverList = CoreService.GetXMLData(path);
            var messages = new StringBuilder();

            if (serverList.Site.Any())
            {
                foreach (var site in serverList.Site)
                {
                    model.Message = site.Name + " is checking status...";
                    Send(model, newStarttime, newEndTime, false, false);
                    Thread.Sleep(5000);

                    foreach (var server in site.Servers)
                    {
                        var result = false;
                        var counter = 0;

                        while (!result && counter < 100)
                        {
                            result = CoreService.Ping(server.Ip);
                            counter++;
                            Thread.Sleep(5000);
                        }

                        messages.Append(" -" + server.Ip + " is " + (result ? "UP" : "DOWN") + " now...");
                        messages.Append("<br/>");
                    }

                    model.Message = messages.ToString();
                    Send(model, newStarttime, newEndTime, false, false);
                    messages.Clear();
                    Thread.Sleep(5000);
                }

                Thread.Sleep(5000);
                model.Message = "All web servers are READY now...<br/>-----------------------------------";
                Send(model, newStarttime, newEndTime);
                pingServerExecuted = false;
            }
        }

        private void RebootServerProcessing(EventModel model, DateTime newStarttime, DateTime newEndTime)
        {
            rebootServerExecuted = true;
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\ProductionServer.xml";
            var serverList = CoreService.GetXMLData(path);
            var messages = new StringBuilder();

            foreach (var site in serverList.Site)
            {
                messages.Append(site.Name + " is rebooting...");
                messages.Append("<br/>");

                foreach (var server in site.Servers)
                {
                    if (server.Waiting != null)
                    {
                        var waitingServerIp = site.Servers.First(x => x.Id == server.Waiting).Ip;
                        var result = false;

                        while (!result)
                        {
                            result = CoreService.Ping(waitingServerIp);
                        }
                    }

                    CoreService.Reboot(server.Ip);

                    messages.Append(" -" + server.Ip + " is rebooting...");
                    messages.Append("<br/>");
                }

                model.Message = messages.ToString();
                Send(model, newStarttime, newEndTime, false, false);
                messages.Clear();
                Thread.Sleep(10000);
            }

            Thread.Sleep(5000);
            model.Message = "All web servers are in the process of restarting...<br/>-----------------------------------";
            Send(model, newStarttime, newEndTime);
            rebootServerExecuted = false;
        }

        private void EnabledSQL(EventModel model, DateTime newStarttime, DateTime newEndTime)
        {
            
            var enabledSites = new List<Site>();

            if (disableAccBySites.Any())
            {
                var sites = CheckSqlAccount(disableAccBySites.ToList());

                disableAccBySites = sites.Where(x => x.Connectionstrings.Any(a => !a.IsEnabled));
                enabledSites = sites.Where(x => x.Connectionstrings.Any(a => a.IsEnabled)).ToList();
            }

            if (enabledSites.Any())
            {
                var messages = new StringBuilder();

                foreach (var enabledSite in enabledSites)
                {
                    messages.Append(enabledSite.Name);
                    messages.Append("<br/>");

                    foreach (var enabledConn in enabledSite.Connectionstrings)
                    {
                        messages.Append("  -Account: " + enabledConn.User + " on " + enabledConn.DataSource + " is ENABLED.");
                        messages.Append("<br/>");
                    }
                }

                model.Message = messages.ToString();

                Send(model, newStarttime, newEndTime, false, false);

                
                Thread.Sleep(10000);
            }
        }

        private void DisabledSQL(EventModel model, DateTime newStarttime, DateTime newEndTime)
        {
            disabledSQLAccountExecuted = true;
            var sites = CheckSqlAccount();

            disableAccBySites = sites.Where(x => x.Connectionstrings.Any(a => !a.IsEnabled));

            if (disableAccBySites.Any())
            {
                var messages = new StringBuilder();
                foreach (var disableSite in disableAccBySites)
                {
                    messages.Append(disableSite.Name);
                    messages.Append("<br/>");

                    foreach (var disableConn in disableSite.Connectionstrings)
                    {
                        messages.Append("  -Account: " + disableConn.User + " on " + disableConn.DataSource + " is DISABLED.");
                        messages.Append("<br/>");
                    }
                }

                model.Message = messages.ToString();

                Send(model, newStarttime, newEndTime);

                disabledSQLAccountExecuted = false;
                Thread.Sleep(10000);
            }
        }

        private List<Site> CheckSqlAccount(List<Site> listSite = null)
        {
            if (listSite == null || !listSite.Any())
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\SQLConnectionChecking.xml";
                listSite = CoreService.GetXMLData(path).Site;
            }

            var result = new List<Site>();
            foreach (var site in listSite)
            {
                var connectionStrings = new List<ConnectionString>();
                foreach (var conn in site.Connectionstrings)
                {
                    if (ConfigurationManager.ConnectionStrings[conn.Name] != null)
                    {
                        connectionStrings.Add(new ConnectionString
                        {
                            Name = conn.Name,
                            DataSource = conn.DataSource,
                            User = conn.User,
                            IsEnabled = OpenConnection(conn.Name)
                        });
                    }
                }

                result.Add(new Site
                {
                    Name = site.Name,
                    Connectionstrings = connectionStrings
                });
            }

            return result;
        }

        private static bool OpenConnection(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            SqlConnection.ClearAllPools();
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State == ConnectionState.Open)
                {
                    return true;
                }
                else
                {
                    try
                    {
                        connection.Open();

                        return connection.State == ConnectionState.Open;
                    }
                    catch
                    {

                        return false;
                    }
                }
            }
        }
    }
}
