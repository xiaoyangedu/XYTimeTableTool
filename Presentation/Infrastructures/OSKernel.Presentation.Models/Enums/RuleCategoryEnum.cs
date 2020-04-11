using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 规则的种类枚举 例：教师规则、课时规则、课程规则
    /// </summary>
    public enum RuleCategoryEnum
    {
        /// <summary>
        /// 教师
        /// </summary>
        [Description("教师")]
        Teacher,
        /// <summary>
        /// 课时
        /// </summary>
        [Description("课时")]
        ClassHour,
        /// <summary>
        /// 课程
        /// </summary>
        [Description("课程")]
        Course,

        /// <summary>
        /// 教室
        /// </summary>
        [Description("教室")]
        Room,
    }
}
