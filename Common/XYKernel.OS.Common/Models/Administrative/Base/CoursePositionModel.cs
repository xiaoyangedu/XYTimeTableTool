using XYKernel.OS.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative
{
    /// <summary>
    /// 课位
    /// </summary>
    public class CoursePositionModel
    {
        /// <summary>
        /// 周次节次
        /// </summary>
        public DayPeriodModel DayPeriod { get; set; }

        /// <summary>
        /// 课位类型枚举
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// 课位顺序
        /// </summary>
        public int PositionOrder { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
