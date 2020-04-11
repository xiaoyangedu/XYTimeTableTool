using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 辅修科目规避时间
    /// </summary>
    public class MinorCourseAvoidTimeRule
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public List<string> CourseIDs { get; set; }

        /// <summary>
        /// 规避时间
        /// </summary>
        public List<DayPeriodModel> AvoidTimes { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}