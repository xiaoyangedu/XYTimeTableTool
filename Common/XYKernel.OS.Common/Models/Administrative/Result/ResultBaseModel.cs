using System.Runtime.Serialization;

namespace XYKernel.OS.Common.Models.Administrative.Result
{
    [DataContract]
    public class ResultClassInfo
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public class ResultCourseInfo
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public class ResultTeacherInfo
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
