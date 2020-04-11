using System.Runtime.Serialization;
using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Mixed.Result
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
        /// class hour teachers
        /// </summary>
        [DataMember]
        public ICollection<string> Teachers { get; set; }

        /// <summary>
        /// 课位
        /// </summary>
        [DataMember]
        public DayPeriodModel DayPeriod { get; set; }
    }
}
