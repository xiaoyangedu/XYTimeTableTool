using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 多个课时有相同的开始日期（日期）
    /// </summary>
    public class ClassHoursSameStartingDayRule: BaseRule
    {
        /// <summary>
        /// 课时ID集合
        /// </summary>
        public int[] ID { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
