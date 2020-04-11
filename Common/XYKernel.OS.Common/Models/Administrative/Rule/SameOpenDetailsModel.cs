using System;
using System.Collections.Generic;
using System.Text;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 同时开课规则
    /// </summary>
    public class SameOpenDetailsModel
    {
        /// <summary>
        /// 课时索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 课程班级
        /// </summary>
        public List<CourseClassModel> Classes { get; set; }
    }

    /// <summary>
    /// 课程班级
    /// </summary>
    public class CourseClassModel
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }
    }
}
