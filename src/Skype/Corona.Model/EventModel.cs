using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corona.Model;

namespace Corona.Models
{  
    public class EventModel
    {
        private DateTime? startTimeValue;

        private DateTime? endTimeValue;
        
        public int Id { get; set; }

        //public DateTime? Start {
        //    get
        //    {
        //        return startTimeValue.HasValue ? startTimeValue.Value.AddHours(AppSettingModel.TimeZone) : startTimeValue;
        //    }

        //    set {
        //        startTimeValue = value;
        //    }
        //}

        //public DateTime? End {
        //    get
        //    {
        //        return endTimeValue.HasValue ? endTimeValue.Value.AddHours(AppSettingModel.TimeZone) : endTimeValue;
        //    }

        //    set
        //    {
        //        endTimeValue = value;
        //    }
        //}

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public string Message { get; set; }

        public Repeat Repeat { get; set; }        

        public bool IsUM { get; set; }

        public bool  IsSent { get; set; }

    }
}