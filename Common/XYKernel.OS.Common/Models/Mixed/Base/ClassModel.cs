using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Mixed
{
    /// <summary>
    /// 班级
    /// </summary>
    public class ClassModel
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 科
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        public string LevelID { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        public List<string> TeacherIDs { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 班额
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        public List<string> StudentIDs { get; set; }
    }
}
