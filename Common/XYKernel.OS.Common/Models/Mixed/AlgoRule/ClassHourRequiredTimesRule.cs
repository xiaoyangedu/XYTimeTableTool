using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 单个课时有多个优先课位
    /// </summary>
    public class ClassHourRequiredTimesRule: BaseRule
    {
        public int ID { get; set; }

        public List<DayPeriodModel> Times { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
