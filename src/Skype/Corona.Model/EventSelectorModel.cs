using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace Corona.Models
{
    public enum Repeat : byte
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }     

    [Serializable]
    public class EventSelectorModel
    {
        [Prompt("Your Event name is: ")]
        public string EventName { get; set; }

        [Prompt("Your conversation ID is: {||}")]
        public string Conversation { get; private set; }

        [Prompt("Your start date is: ")]
        public DateTime StartDate { get; set; }

        [Prompt("Your message is: ")]
        public string Message { get; set; }

        [Prompt("Your schedule option is: {||}")]
        public Repeat? Repeat { get; set; }                
    }
}