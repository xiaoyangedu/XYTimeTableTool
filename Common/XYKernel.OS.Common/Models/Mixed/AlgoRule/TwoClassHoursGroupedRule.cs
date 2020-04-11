using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 给2个课时分组
    /// </summary>
    public class TwoClassHoursGroupedRule: BaseRule
    {
        public int FirstID { get; set; }

        public int SecondID { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
