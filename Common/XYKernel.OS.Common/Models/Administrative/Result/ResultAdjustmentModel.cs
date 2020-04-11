using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XYKernel.OS.Common.Models.Administrative.Result
{
    [DataContract]
    public class ResultAdjustmentModel
    {
        [DataMember]
        public ICollection<ClassHourAdjustmentModel> ClassHourAdjustmentDetails { get; set; }

        [DataMember]
        public ResultModel CurrentResult { get; set; }

        [DataMember]
        public string ResultId { get; set; }
    }
}
