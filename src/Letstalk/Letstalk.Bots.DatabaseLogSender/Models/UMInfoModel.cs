using System;

namespace Letstalk.Bots.DatabaseLogSender.Models
{
    public class UMInfoModel
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public bool IsUnderMaintenanceTime { get; set; }

        public int ErrorCode { get; set; }
    }
}
