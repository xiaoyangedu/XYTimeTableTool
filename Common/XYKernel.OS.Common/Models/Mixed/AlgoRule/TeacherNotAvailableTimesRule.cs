using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 教师的不可用时间
    /// </summary>
    public class TeacherNotAvailableTimesRule:BaseRule
    {
        public string TeacherID { get; set; }

        /// <summary>
        /// 不可用时间集合
        /// </summary>
        public List<DayPeriodModel> Times { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
