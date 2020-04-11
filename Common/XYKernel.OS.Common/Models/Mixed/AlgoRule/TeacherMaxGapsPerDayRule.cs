using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 教师每天最大课程间隔
    /// </summary>
    public class TeacherMaxGapsPerDayRule : BaseRule
    {
        /// <summary>
        /// 教师ID
        /// </summary>
        public string TeacherID { get; set; }

        /// <summary>
        /// 最大课程间隔
        /// </summary>
        public int MaxGaps { get; set; }
    }
}
