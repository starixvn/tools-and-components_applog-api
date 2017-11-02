using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corona.Models;

namespace Corona.Models
{  
    public class EventModel
    {
        public int Id { get; set; }               

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public string Message { get; set; }

        public Repeat Repeat { get; set; }        

        public bool IsUM { get; set; }

        public bool  IsSent { get; set; }

        public int GMT { get; set; }

        public string MessageFile { get; set; }
    }
}