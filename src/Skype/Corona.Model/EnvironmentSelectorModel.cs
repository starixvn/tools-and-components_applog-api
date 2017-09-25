using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace Corona.Models
{
    [Serializable]
    public class EnvironmentSelectorModel
    {
        [Prompt("Which environment do you like (Local, UAT, SIT, Pro) ?")]
        public string Environment { get; set; }
    }
}