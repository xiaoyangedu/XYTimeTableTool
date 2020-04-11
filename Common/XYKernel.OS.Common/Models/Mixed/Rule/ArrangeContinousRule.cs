using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Mixed.Rule
{
    /// <summary>
    /// 课时连排
    /// </summary>
    public class ArrangeContinousRule
    {
        /// <summary>
        /// 班级
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 连排数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 是否间隔连排
        /// </summary>
        public bool IsIntervalDay { get; set; }

        /// <summary>
        /// 日期-节次
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

        /// <summary>
        /// 隔天连排权重
        /// </summary>
        public int IntervalDayWeight { get; set; }
    }
}
