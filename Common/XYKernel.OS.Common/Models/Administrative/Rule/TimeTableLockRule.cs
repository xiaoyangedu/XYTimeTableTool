using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 锁定课表
    /// </summary>
    public class TimeTableLockRule
    {
        /// <summary>
        /// 锁定课位
        /// </summary>
        public List<LockedClassTimeTable> LockedTimeTable { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }

    /// <summary>
    /// 锁定课位信息
    /// </summary>
    public class LockedClassTimeTable
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 锁定课程集合
        /// </summary>
        public List<LockedClassCourseTimeTable> LockedCourseTimeTable { get; set; }
    }

    /// <summary>
    /// 锁定课程项
    /// </summary>
    public class LockedClassCourseTimeTable
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 锁定区域
        /// </summary>
        public List<DayPeriodModel> LockedTimes { get; set; }
    }
}