using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.Models
{
    public static class FunctionKeyModel
    {        
        public const string UnderMaintenace = "UM";
        public const string Morning = "Morning";        
    }

    public static class EventKeyModel
    {
        public const string UMStarted = "UM.Started";
        public const string UMNotified = "UM.Notified";
        public const string UMExtended = "UM.Extended";
        public const string UMEnded = "UM.Ended";
        public const string Morning = "Morning";
        public const string UMDisabledSQL = "UM.DisabledSQL";
        public const string UMEnabledSQL = "UM.EnabledSQL";
        public const string UMRebootServer = "UM.Reboot.Server";
        public const string UMRebootServerProcessing = "UM.Reboot.Server.Processing";
    }
}
