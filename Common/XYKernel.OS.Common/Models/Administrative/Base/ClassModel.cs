using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative
{
    /// <summary>
    /// 班级模型
    /// </summary>
    public class ClassModel
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 课程设置
        /// </summary>
        public List<ClassCourseModel> Settings { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        public List<StudentModel> Students { get; set; }

        public ClassModel()
        {
            this.Settings = new List<ClassCourseModel>();
            this.Students = new List<StudentModel>();
        }
    }
}
