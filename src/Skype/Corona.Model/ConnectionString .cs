using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Corona.Models
{
    [XmlRoot(ElementName = "connectionstring")]
    public class ConnectionString
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "user")]
        public string User { get; set; }

        [XmlAttribute(AttributeName = "datasource")]
        public string DataSource { get; set; }

        public bool IsEnabled { get; set; }
    }
}
