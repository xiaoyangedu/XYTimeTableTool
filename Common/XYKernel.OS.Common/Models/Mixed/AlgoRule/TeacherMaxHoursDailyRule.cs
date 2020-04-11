using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    /// <summary>
    /// 教师每天最大课时数
    /// </summary>
    public class TeacherMaxHoursDailyRule: BaseRule
    {
        public string TeacherID { get; set; }

        public int MaxHours { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
