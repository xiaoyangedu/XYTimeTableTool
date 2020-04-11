using XYKernel.OS.Common.Enums;

namespace XYKernel.OS.Common.Models.Mixed
{
    /// <summary>
    /// 走班课位
    /// </summary>
    public class CoursePositionModel
    {
        /// <summary>
        /// 周次节次
        /// </summary>
        public DayPeriodModel DayPeriod { get; set; }

        /// <summary>
        /// 课位类型
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
