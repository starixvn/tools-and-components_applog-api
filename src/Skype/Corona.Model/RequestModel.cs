using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Models;

namespace Corona.Model
{
    public class RequestModel
    {        
        public IList<EventModel> Events { get; set; }

        public string Message { get; set; }
    }
}
