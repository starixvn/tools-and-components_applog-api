using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.Models
{
    public class ScheduleModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime  StartDate { get; set; }

        public DateTime  EndDate { get; set; }

        public Repeat Repeat { get; set; }

        public int GMT { get; set; }
    }
}
