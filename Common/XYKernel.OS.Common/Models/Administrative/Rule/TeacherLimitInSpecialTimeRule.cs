using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 教师在特定课位的课时限制(总的限制及到班的限制)
    /// </summary>
    public class TeacherLimitInSpecialTimeRule
    {
        /// <summary>
        /// 教师ID
        /// </summary>
        public List<string> TeacherIDs { get; set; }

        /// <summary>
        /// 特殊时间
        /// </summary>
        public List<DayPeriodModel> SpecialTimes { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }

    }
}