using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 给2个课时分组
    /// </summary>
    public class TwoClassHoursGroupedRule: BaseRule
    {
        /// <summary>
        /// 第一个课时
        /// </summary>
        public int FirstID { get; set; }

        /// <summary>
        /// 第二个课时
        /// </summary>
        public int SecondID { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
