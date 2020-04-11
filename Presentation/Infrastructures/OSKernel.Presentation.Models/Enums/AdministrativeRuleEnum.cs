using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 行政班规则枚举
    /// </summary>
    public enum AdministrativeRuleEnum
    {
        /// <summary>
        /// 课程互斥
        /// </summary>
        [Description("课程互斥")]
        MutexGroup,
        /// <summary>
        /// 合班上课
        /// </summary>
        [Description("合班上课")]
        ClassUnion,
        /// <summary>
        /// 教师排课时间
        /// </summary>
        [Description("教师排课时间")]
        TeacherTime,
        /// <summary>
        /// 同时开课限制
        /// </summary>
        [Description("同时开课限制")]
        CourseLimit,
        /// <summary>
        /// 师徒跟随
        /// </summary>
        [Description("师徒跟随")]
        MasterApprenttice,
        /// <summary>
        /// 教案齐头
        /// </summary>
        [Description("教案齐头")]
        TeachingPlanFlush,
        /// <summary>
        /// 课时分散
        /// </summary>
        [Description("课时分散")]
        ClassHourAverage,
        /// <summary>
        /// 同时开课
        /// </summary>
        [Description("同时开课")]
        ClassHourSameOpen,
        /// <summary>
        /// 上下午课时
        /// </summary>
        [Description("上下午课时")]
        AmPmClassHour,
        /// <summary>
        /// 单双周
        /// </summary>
        [Description("单双周")]
        OddDualWeek,
        /// <summary>
        /// 课程连排
        /// </summary>
        [Description("课程连排")]
        CourseArrangeContinuous,
        /// <summary>
        /// 课程排课时间
        /// </summary>
        [Description("课程排课时间")]
        CourseTime,
        /// <summary>
        /// 连排齐头
        /// </summary>
        [Description("连排齐头")]
        ArrangeContinuousPlanFlush,
        /// <summary>
        /// 教师每天最大课时数
        /// </summary>
        [Description("教师每天最大课时数")]
        TeacherMaxHoursDaily,
        /// <summary>
        /// 教师每周最大工作天数
        /// </summary>
        [Description("教师每周最大工作天数")]
        TeacherMaxDaysPerWeek,
        /// <summary>
        /// 教师每天最大课时间隔
        /// </summary>
        [Description("教师每天最大课时间隔")]
        TeacherMaxGapsPerDay,
        /// <summary>
        /// 教师上下午不连排
        /// </summary>
        [Description("教师上下午不连排")]
        TeacherAmPmNoContinues,
        [Description("教师半天上课")]
        TeacherHalfDayWorkRule,
        [Description("教案平头")]
        TeacherPriorityBalanceRule,
        [Description("锁定课表")]
        LockedCourse
    }
}
