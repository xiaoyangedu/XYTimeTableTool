using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 多个课时有相同的开始时间（日期+时间）
    /// </summary>
    public class ClassHoursSameStartingTimeRule: BaseRule
    {
        /// <summary>
        /// 课时ID
        /// </summary>
        public int[] Id { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
