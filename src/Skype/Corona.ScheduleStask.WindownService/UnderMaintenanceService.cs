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

namespace Corona.ScheduleTask.WindownService
{
    public class UnderMaintenanceService : BaseService
    {
        private readonly IDictionary<string, bool> connectionStringNames;
        public UnderMaintenanceService()
        {
            connectionStringNames = new Dictionary<string, bool> {
                {"DBCust.CustDB.bodbAdminNet", false },
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
                    var schedule = schedules.First(x => x.Id == model.Schedule.Id);
                    var oldStartTime = Utilities.DatetimeConverter(schedule.StartDate);
                    var oldEndTime = Utilities.DatetimeConverter(schedule.EndDate);
                    var newStarttime = Utilities.DatetimeConverter(umConfig.From);
                    var newEndTime = Utilities.DatetimeConverter(umConfig.To);
                    var isUM = umConfig.IsUM != -1 ? umConfig.IsUM == 1 : UnderMaintenance().IsUM == 1;

                    switch (model.KeyName)
                    {
                        case EventKeyModel.UMNotified:
                            {
                                if (!isUM && oldStartTime != newStarttime && oldEndTime != newEndTime)
                                {
                                    Send(model, newStarttime, newEndTime, true);
                                }

                                break;
                            }
                        case EventKeyModel.UMStarted:
                            {
                                if (isUM)
                                {
                                    Send(model, oldStartTime, oldEndTime, true);
                                }

                                break;
                            }
                        case EventKeyModel.UMExtended:
                            {
                                if (isUM && oldEndTime != newEndTime)
                                {
                                    Send(model, oldStartTime, newEndTime, true);
                                }

                                break;
                            }
                        default:
                            {
                                if (!isUM)
                                {
                                    Send(model, oldStartTime, oldEndTime, true);
                                }

                                break;
                            }
                    }
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

           return connectionStatus.Values.First(x => !x);            
        }

        private static bool OpenConnection(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;

                if (connection.State == ConnectionState.Open)
                {
                    return true;
                }
                else
                {
                    try
                    {
                        connection.Open();

                        return true;
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
