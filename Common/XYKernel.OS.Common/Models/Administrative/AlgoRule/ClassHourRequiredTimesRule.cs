using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 单个课时有多个优先课位
    /// </summary>
    public class ClassHourRequiredTimesRule : BaseRule
    {
        /// <summary>
        /// 课时ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 时间集合
        /// </summary>
        public List<DayPeriodModel> Times { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
