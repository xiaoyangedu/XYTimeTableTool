using System.Runtime.Serialization;
using System.Collections.Generic;
using XYKernel.OS.Common.Enums;

namespace XYKernel.OS.Common.Models.Administrative.Result
{
    [DataContract]
    public class ResultDetailModel
    {
        /// <summary>
        /// 课时Id
        /// </summary>
        [DataMember]
        public int ClassHourId { get; set; }

        /// <summary>
        /// CourseID
        /// </summary>
        [DataMember]
        public string CourseID { get; set; }

        /// <summary>
        /// 课位
        /// </summary>
        [DataMember]
        public DayPeriodModel DayPeriod { get; set; }

        /// <summary>
        /// class hour teachers
        /// </summary>
        [DataMember]
        public ICollection<string> Teachers { get; set; }

        /// <summary>
        /// 课时类型，常规还是单双周
        /// </summary>
        [DataMember]
        public ClassHourResultType ResultType { get; set; } = ClassHourResultType.Normal;
    }
}
