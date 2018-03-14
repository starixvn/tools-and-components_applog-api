using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Corona.Models
{
    [XmlRoot(ElementName = "server")]
    public class Server
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "ip")]
        public string Ip { get; set; }

        [XmlAttribute(AttributeName = "waiting")]
        public string Waiting { get; set; }
    }
}
