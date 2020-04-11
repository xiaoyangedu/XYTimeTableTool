using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XYKernel.OS.Common.Models.Administrative.Result
{
    [DataContract]
    public class ResultModel
    {
        [DataMember]
        public ICollection<ResultClassModel> ResultClasses { get; set; }

        [DataMember]
        public ICollection<ResultClassInfo> Classes { get; set; }

        [DataMember]
        public ICollection<ResultCourseInfo> Courses { get; set; }

        [DataMember]
        public ICollection<ResultTeacherInfo> Teachers { get; set; }

        [DataMember]
        public ICollection<CoursePositionModel> Positions { get; set; }
    }
}
