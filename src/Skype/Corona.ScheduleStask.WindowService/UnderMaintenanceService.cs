using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Core;
using Corona.Models;

namespace Corona.ScheduleTask.WindowService
{
    public class UnderMaintenanceService : BaseService
    {
        private readonly IDictionary<string, bool> connectionStringNames;
        public UnderMaintenanceService()
        {
            connectionStringNames = new Dictionary<string, bool> {
                { "DBCust.CustDB.bodbAdminNet", false },
                { "DBPRO.bodb02.bodbAdminNet", false },
                { "DBTXN.bodb02.bodbAdminNet", false}
            };


        }
        public void ExecuteUMTask(EventModel2 model)
        {
            if (model != null)
            {
                var umConfig = BaseService.UMConfig();
                var schedules = GetSchedules();

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
                    UMProcessing(model, umConfig, schedules);
                }
            }
        }

        private void UMProcessing(EventModel2 model, UnderMaintenanceModel umConfig, List<ScheduleModel> schedules)
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
                        var isDisabled = CheckSqlDisabled();

                        if (isDisabled)
                        {
                            Send(model, newStarttime, newEndTime);
                        }
                        break;
                    }
                case EventKeyModel.UMEnabledSQL:
                    {
                        var isDisabled = CheckSqlDisabled();

                        if (isUM && !isDisabled)
                        {
                            Send(model, newStarttime, newEndTime);
                        }

                        break;
                    }                   
                case EventKeyModel.UMRebootServer:
                    {
                        var rebootTime = newStarttime.AddMinutes(1);
                        var currentTime = DateTime.UtcNow.AddHours(timeZone);

                        if (isUM && Utilities.DatetimeComparer(rebootTime, currentTime) == 0)
                        {
                            Send(model, newStarttime, newEndTime);
                        }

                        break;
                    }
                case EventKeyModel.UMRebootServerProcessing:
                    {
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

        public bool CheckSqlDisabled()
        {
            var connectionStatus = new Dictionary<string, bool>();

            connectionStringNames.ToList().ForEach(x =>
            {
                connectionStatus.Add(x.Key, OpenConnection(x.Key));
            });

            return connectionStatus.Values.Any(x => x == false);
        }

        private static bool OpenConnection(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

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
