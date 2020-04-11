using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 合班上课
    /// </summary>
    public class ClassUnionRule
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 班级ID集合
        /// </summary>
        public List<string> ClassIDs { get; set; }
    }
}
