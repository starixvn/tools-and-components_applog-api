using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Corona.Models
{
    [XmlRoot(ElementName = "sites")]
   public class Sites
    {
        [XmlElement(ElementName = "site")]
        public List<Site> Site { get; set; }
    }
}
