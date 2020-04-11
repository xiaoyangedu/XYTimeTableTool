using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 多个课时之间的最大间隔天数
    /// </summary>
    public class MaxDaysBetweenClassHoursRule: BaseRule
    {
        /// <summary>
        /// 课时ID集合
        /// </summary>
        public int[] ID { get; set; }

        /// <summary>
        /// 最大天数
        /// </summary>
        public int MaxDays { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
