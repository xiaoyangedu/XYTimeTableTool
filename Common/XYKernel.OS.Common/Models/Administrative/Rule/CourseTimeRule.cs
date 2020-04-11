using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 课程必须禁止时间
    /// </summary>
    public class CourseTimeRule
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 必须时间
        /// </summary>
        public List<DayPeriodModel> MustTimes { get; set; }

        /// <summary>
        /// 禁止时间
        /// </summary>
        public List<DayPeriodModel> ForbidTimes { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
