using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 教师上下午不连排
    /// </summary>
    public class TeacherAmPmNoContinousRule
    {
        /// <summary>
        /// 教师ID
        /// </summary>
        public string TeacherID { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
