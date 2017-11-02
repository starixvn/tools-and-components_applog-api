using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.Models
{
    public class UnderMaintenanceModel
    {
        public int IsUM { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

    }
}
