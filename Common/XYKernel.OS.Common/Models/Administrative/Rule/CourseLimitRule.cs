using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 同时开课限制
    /// </summary>
    public class CourseLimitRule
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 班级数
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        public List<PeriodLimit> PeriodLimits { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }

    public class PeriodLimit
    {
        public int Limit { get; set; }

        public DayPeriodModel DayPeriod { get; set; }
    }
}
