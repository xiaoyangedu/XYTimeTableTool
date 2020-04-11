using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative
{
    /// <summary>
    /// 课时
    /// </summary>
    public class ClassHourModel
    {
        public int ID { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 教师IDs
        /// </summary>
        public List<string> TeacherIDs { get; set; }

    }
}
