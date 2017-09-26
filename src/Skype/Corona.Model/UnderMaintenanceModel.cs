using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.Model
{
    public class UnderMaintenanceModel
    {
        public bool IsUM { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

    }
}
