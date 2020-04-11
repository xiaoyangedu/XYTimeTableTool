using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 多个课时有多个优先课位
    /// </summary>
    public class ClassHoursRequiredTimesRule:BaseRule
    {
        /// <summary>
        /// 教师ID
        /// </summary>
        public string TeacherID { get; set; }
        /// <summary>
        /// 学生ID
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }
        /// <summary>
        /// 标签ID
        /// </summary>
        public string TagID { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }
        /// <summary>
        /// 课位
        /// </summary>
        public List<DayPeriodModel> Times { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
