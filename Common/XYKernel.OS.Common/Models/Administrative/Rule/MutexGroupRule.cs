using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 课程互斥规则
    /// </summary>
    public class MutexGroupRule
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public List<string> CourseIDs { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
