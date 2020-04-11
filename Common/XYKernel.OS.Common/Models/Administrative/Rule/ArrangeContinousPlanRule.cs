using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 连排齐头
    /// </summary>
    public class ArrangeContinousPlanRule
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 教师ID
        /// </summary>
        public string TeacherID { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public List<string> ClassIDs { get; set; }

        /// <summary>
        /// 几日内齐头
        /// </summary>
        public int FlushDays { get; set; }
    }
}
