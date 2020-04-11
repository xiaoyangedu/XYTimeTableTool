using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 多个课时间最小课程间隔天数
    /// </summary>
    public class MinDaysBetweenClassHoursRule:BaseRule
    {
        public int[] ID { get; set; }

        public int MinDays { get; set; }

        public bool ConsecutiveIfSameDay { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
