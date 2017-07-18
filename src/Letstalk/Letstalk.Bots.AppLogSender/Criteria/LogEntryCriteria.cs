using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data.Repository;

namespace Letstalk.Bots.AppLogSender.Criteria
{
    public class LogEntryCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public bool IsProduction { get; set; }

        public override string GetSettingKey()
        {
            return "IS_SelAppLog_All_V2";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
