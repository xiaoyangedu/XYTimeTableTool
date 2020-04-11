using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 多个课时在选定课位中占用的最大数量
    /// </summary>
    public class ClassHoursOccupyMaxTimeFromSelectionRule: BaseRule
    {
        /// <summary>
        /// 课时ID集合
        /// </summary>
        public int[] ID { get; set; }

        /// <summary>
        /// 最大数值
        /// </summary>
        public int MaxOccupyNumber { get; set; }

        /// <summary>
        /// 课位
        /// </summary>
        public List<DayPeriodModel> Times { get; set; }
    }
}
