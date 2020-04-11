using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 多个课时在选定课位中占用的最大数量
    /// </summary>
    public class ClassHoursOccupyMaxTimeFromSelectionRule:BaseRule
    {
        public int[] ID { get; set; }

        public int MaxOccupyNumber { get; set; }

        // 课位
        public List<DayPeriodModel> Times { get; set; }
    }
}
