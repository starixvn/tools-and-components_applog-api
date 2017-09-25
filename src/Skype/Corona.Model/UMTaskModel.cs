using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace Corona.Models
{
    public enum UMTaskList
    {        
        UMInfo,
        RebootServer,
        ShutdownServer
    };

    [Serializable]
    public class UMTaskModel
    {
        [Prompt("What kind of UM task would you like? {||}")]
        public UMTaskList? UMTaskList { get; set; }
    }
}