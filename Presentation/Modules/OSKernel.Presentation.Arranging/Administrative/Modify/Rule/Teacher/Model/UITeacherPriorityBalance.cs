using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Rule;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Model
{
    public class UITeacherPriorityBalance : UIRuleBase
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
        /// 教师
        /// </summary>
        public string Teacher { get; set; }

        /// <summary>
        /// 齐头班级
        /// </summary>
        public List<UIClass> Classes { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public List<UICourse> Courses { get; set; }

        /// <summary>
        /// 课程字符串
        /// </summary>
        public string CourseString
        {
            get
            {
                return Courses?.Select(c => c.Name)?.Parse();
            }
        }

        /// <summary>
        /// 班级字符串
        /// </summary>
        public string ClassString
        {
            get
            {
                return Classes?.Select(c => c.Name)?.Parse();
            }
        }
    }
}
