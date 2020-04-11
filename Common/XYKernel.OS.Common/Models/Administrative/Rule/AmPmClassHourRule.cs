using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 上下午课时
    /// </summary>
    public class AmPmClassHourRule
    {
        /// <summary>
        /// 班级
        /// </summary>
        public string ClassID { get; set; }
        /// <summary>
        /// 课程
        /// </summary>
        public string CourseID { get; set; }
        /// <summary>
        /// 上午最大课时
        /// </summary>
        public int AmMax { get; set; }
        /// <summary>
        /// 下午最大课时
        /// </summary>
        public int PmMax { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
