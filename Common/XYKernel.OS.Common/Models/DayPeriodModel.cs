using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models
{
    /// <summary>
    /// 日期-节次
    /// </summary>
    public class DayPeriodModel
    {
        [DataMember]
        public DayOfWeek Day { get; set; }
        [DataMember]
        public int Period { get; set; }
        [DataMember]
        public string PeriodName { get; set; }
    }
}
