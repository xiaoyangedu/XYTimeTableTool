using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 所有教师每周最大工作天数
    /// </summary>
    public class TeachersMaxDaysPerWeekRule: BaseRule
    {
        /// <summary>
        /// 最大天数
        /// </summary>
        public int MaxDays { get; set; }
    }
}
