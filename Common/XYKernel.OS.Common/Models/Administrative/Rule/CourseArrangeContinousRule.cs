using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 课程连排
    /// </summary>
    public class CourseArrangeContinousRule
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 连排数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 是否隔天
        /// </summary>
        public bool IsIntervalDay { get; set; }

        /// <summary>
        /// 隔天连排权重
        /// </summary>
        public int IntervalDayWeight { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public List<DayPeriodModel> Times { get; set; }

        /// <summary>
        /// 连排优先时间权重
        /// </summary>
        public int TimesWeight { get; set; }

        /// <summary>
        /// 连排不跨上下午，大课间
        /// </summary>
        public bool NoCrossingBreak { get; set; }

        /// <summary>
        /// 连排不跨上下午，大课间权重
        /// </summary>
        public int NoCrossingBreakWeight { get; set; }
    }
}
