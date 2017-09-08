using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Corona.Models
{
    public class EventModel
    {
        public string Id { get; set; }

        public DateTime Start { get; set; }

        public DateTime  End { get; set; }

        public string Message { get; set; }
    }
}