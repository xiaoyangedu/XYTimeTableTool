using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 教师每周最大工作天数
    /// </summary>
    public class TeacherMaxDaysPerWeekRule: BaseRule
    {
        public string TeacherID { get; set; }

        /// <summary>
        /// 最大天数
        /// </summary>
        public int MaxDays { get; set; }
    }
}
