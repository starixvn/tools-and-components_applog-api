using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.Model
{
   public class AppSettingModel
    {
        public static int TimeZone => Convert.ToInt32(ConfigurationManager.AppSettings["TimeZone"]);
    }
}
