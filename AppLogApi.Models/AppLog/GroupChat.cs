using System.Xml.Serialization;

namespace AppLogApi.Models.AppLog
{
    public class GroupChat
    {
        public string LogCategory { get; set; }

        public string Targets { get; set; }

        [XmlAttribute]
        public bool IsIgnore { get; set; }
    }
}