using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 主修科目优先时间及排课占比
    /// </summary>
    public class MajorCoursePreferredTimeRule
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public List<string> ClassIDs { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public List<string> CourseIDs { get; set; }

        /// <summary>
        /// 优先时间
        /// </summary>
        public List<DayPeriodModel> PreferredTimes { get; set; }

        /// <summary>
        /// 优先时间课时占比
        /// </summary>
        public int Percent { get; set; }

        /// <summary>
        /// 规则权重
        /// </summary>
        public int Weight { get; set; }

    }
}