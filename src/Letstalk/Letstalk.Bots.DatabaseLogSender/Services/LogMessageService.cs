using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Letstalk.Bots.DatabaseLogSender.Models;

namespace Letstalk.Bots.DatabaseLogSender.Services
{
    public interface ILogmessageService
    {
        IList<LogEntry> GetLatestData();
    }

    public class LogMessageService : ILogmessageService
    {      
        public IList<LogEntry> GetLatestData()
        {
            var criteria = new LogMessageCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<LogEntry>().ToList();
            }            
        }
    }

    public class LogMessageCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "dba_Letstalk_Notify";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
