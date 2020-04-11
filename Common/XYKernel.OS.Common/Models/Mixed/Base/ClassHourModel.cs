using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed
{
    /// <summary>
    /// 走班课时
    /// </summary>
    public class ClassHourModel
    {
        /// <summary>
        /// 课时ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 层ID
        /// </summary>
        public string LevelID { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> TagIDs { get; set; }

        /// <summary>
        /// 教师ID
        /// </summary>
        public List<string> TeacherIDs { get; set; }

    }
}
