using System.Runtime.Serialization;

namespace Corona.Models
{
    [DataContract]
    public class FunctionModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Key { get; set; }
    }
}
