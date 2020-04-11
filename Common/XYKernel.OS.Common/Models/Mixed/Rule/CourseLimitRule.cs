using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Mixed.Rule
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
        /// 课位限制
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

    /// <summary>
    /// 课位限制类
    /// </summary>
    public class PeriodLimit
    {
        /// <summary>
        /// 限制数
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 课位
        /// </summary>
        public DayPeriodModel DayPeriod { get; set; }
    }
}
