using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative
{
    /// <summary>
    /// 课程设置
    /// </summary>
    public class ClassCourseModel
    {
        /// <summary>
        /// 科目ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public int Lessons { get; set; }

        /// <summary>
        /// 教师ID集合
        /// </summary>
        public List<string> TeacherIDs { get; set; }
    }
}
