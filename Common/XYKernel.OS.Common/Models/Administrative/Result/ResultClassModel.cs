using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XYKernel.OS.Common.Models.Administrative.Result
{
    [DataContract]
    public class ResultClassModel
    {
        /// <summary>
        /// ClassID
        /// </summary>
        [DataMember]
        public string ClassID { get; set; }

        /// <summary>
        /// Students of class
        /// </summary>
        [DataMember]
        public ICollection<string> ResultStudents { get; set; }

        /// <summary>
        /// Class Hours
        /// </summary>
        [DataMember]
        public ICollection<ResultDetailModel> ResultDetails { get; set; }
    }
}
