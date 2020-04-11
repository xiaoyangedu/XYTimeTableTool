using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 单个课时有一个优先开始时间
    /// </summary>
    public class ClassHourRequiredStartingTimeRule: BaseRule
    {
        public int ID { get; set; }

        /// <summary>
        /// 周期
        /// </summary>
        public DayPeriodModel Period { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
