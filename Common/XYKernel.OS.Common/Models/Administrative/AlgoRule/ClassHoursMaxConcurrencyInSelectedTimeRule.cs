using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 在选定课位中设置多个课时的最大同时开课数量
    /// </summary>
    public class ClassHoursMaxConcurrencyInSelectedTimeRule: BaseRule
    {
        /// <summary>
        /// 
        /// </summary>
        public int[] ID { get; set; }

        /// <summary>
        /// 课位
        /// </summary>
        public List<DayPeriodModel> Times { get; set; }

        /// <summary>
        /// 最大数值
        /// </summary>
        public int MaxConcurrencyNumber { get; set; }
    }
}
