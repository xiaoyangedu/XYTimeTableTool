using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 所有教师每天最大课程间隔
    /// </summary>
    public class TeachersMaxGapsPerDayRule: BaseRule
    {
        /// <summary>
        /// 最大间隔
        /// </summary>
        public int MaxGaps { get; set; }
    }
}
