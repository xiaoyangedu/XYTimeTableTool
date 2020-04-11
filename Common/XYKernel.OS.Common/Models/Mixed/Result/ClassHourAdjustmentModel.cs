using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using XYKernel.OS.Common.Enums;

namespace XYKernel.OS.Common.Models.Mixed.Result
{
    [DataContract]
    public class ClassHourAdjustmentModel
    {
        [DataMember]
        public List<AdjustmentDetailModel> Start { get; set; }

        [DataMember]
        public List<AdjustmentDetailModel> End { get; set; }

        [DataMember]
        public DateTime AdjustmentTime { get; set; }

        [DataMember]
        public AdjustTypeEnum AdjustType { get; set; }
    }

    [DataContract]
    public class AdjustmentDetailModel
    {
        [DataMember]
        public string CourseName { get; set; }

        [DataMember]
        public string LevelName { get; set; }

        [DataMember]
        public string ClassName { get; set; }

        [DataMember]
        public List<string> TeacherNames { get; set; }

        [DataMember]
        public DayPeriodModel TimeSlot { get; set; }
    }
}
