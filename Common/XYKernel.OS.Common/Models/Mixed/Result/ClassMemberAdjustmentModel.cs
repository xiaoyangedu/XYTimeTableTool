using System;
using System.Runtime.Serialization;

namespace XYKernel.OS.Common.Models.Mixed.Result
{
    [DataContract]
    public class ClassMemberAdjustmentModel
    {
        [DataMember]
        public string StudentID { get; set; }

        [DataMember]
        public string FromClassID { get; set; }

        [DataMember]
        public string ToClassID { get; set; }

        [DataMember]
        public DateTime AdjustmentTime { get; set; }
    }
}
