using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Models;

namespace Corona.Models
{
    public class RequestModel
    {        
        public IList<EventModel> Events { get; set; }

        public EventModel2 Event2 { get; set; }

        public string Message { get; set; }

        public bool IsUpdateData { get; set; }
    }
}
