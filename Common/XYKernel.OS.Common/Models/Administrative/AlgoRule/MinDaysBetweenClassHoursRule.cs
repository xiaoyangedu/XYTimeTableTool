using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 多个课时间最小课程间隔天数
    /// </summary>
    public class MinDaysBetweenClassHoursRule: BaseRule
    {
        /// <summary>
        /// 课时集合ID
        /// </summary>
        public int[] ID { get; set; }

        /// <summary>
        /// 最小天数
        /// </summary>
        public int MinDays { get; set; }

        /// <summary>
        /// 同一天连排
        /// </summary>
        public bool ConsecutiveIfSameDay { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
