using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.Rule
{
    /// <summary>
    /// 上下午课时
    /// </summary>
    public class AmPmClassHourRule
    {
        /// <summary>
        /// 科目ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 层ID
        /// </summary>
        public string LevelID { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 上午最大
        /// </summary>
        public int AmMax { get; set; }

        /// <summary>
        /// 下午最大
        /// </summary>
        public int PmMax { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
