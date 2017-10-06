using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corona.Models;

namespace Corona.Model
{
    public class UMRequestModel
    {
        public bool IsNotification { get; set; }

        public bool IsExtend { get; set; }

        public bool IsUM { get; set; }

        public EventModel Events { get; set; }
    }
}
