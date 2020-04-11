using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 单个课时有多个优先开始时间
    /// </summary>
    public class ClassHourRequiredStartingTimesRule : BaseRule
    {
        /// <summary>
        /// 课时ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 课位
        /// </summary>
        public List<DayPeriodModel> Times { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
