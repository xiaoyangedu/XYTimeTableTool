using System.Runtime.Serialization;
using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Mixed.Result
{
    [DataContract]
    public class ResultClassInfo
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string CourseID { get; set; }

        [DataMember]
        public string LevelID { get; set; }

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

        [DataMember]
        public List<ResultLevelInfo> Levels { get; set; }
    }

    [DataContract]
    public class ResultLevelInfo
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

    [DataContract]
    public class ResultStudentInfo
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
