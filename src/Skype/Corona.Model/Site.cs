using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Corona.Models
{
    [XmlRoot(ElementName = "site")]
    public class Site
    {
        [XmlElement(ElementName = "server")]
        public List<Server> Servers { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "connectionstring")]
        public List<ConnectionString> Connectionstrings { get; set; }
    }
}
