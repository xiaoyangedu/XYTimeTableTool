using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 教师每天最大课时数
    /// </summary>
    public class TeacherMaxHoursDailyRule
    {
        /// <summary>
        /// 教师ID
        /// </summary>
        public string TeacherID { get; set; }

        /// <summary>
        /// 最大课时数
        /// </summary>
        public int MaxHour { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
