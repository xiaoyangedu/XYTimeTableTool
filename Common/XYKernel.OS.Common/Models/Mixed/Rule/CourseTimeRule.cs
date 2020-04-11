using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Mixed.Rule
{
    /// <summary>
    /// 课程排课时间
    /// </summary>
    public class CourseTimeRule
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

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
