using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.Models
{
    public class EventModel2
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }       

        public FunctionModel Functions { get; set; }

        public ScheduleModel  Schedule { get; set; }

        public bool IsSent { get; set; }

        public IList<EventModel2> Child { get; set; }

        public int ParentId { get; set; }

        public string MessageFile { get; set; }

        public string KeyName { get; set; }
    }
}
