using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 单双周
    /// </summary>
    public class OddDualWeekRule
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 单周课程
        /// </summary>
        public string OddWeekCourseID { get; set; }

        /// <summary>
        /// 双周课程
        /// </summary>
        public string DualWeekCourseID { get; set; }
    }
}
