using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 多个课时有多个优先开始时间
    /// </summary>
    public class ClassHoursRequiredStartingTimesRule:BaseRule
    {
        /// <summary>
        /// 教师
        /// </summary>
        public string TeacherID { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        public string Students { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string TagID { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 时间集合
        /// </summary>
        public List<DayPeriodModel> Times { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
