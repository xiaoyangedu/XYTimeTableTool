using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 走班算法枚举
    /// </summary>
    public enum MixedAlgoRuleEnum
    {
        /// <summary>
        /// 多个课时有相同的开始日期（日期）
        /// </summary>
        [Description("多个课时有相同的开始日期（日期）")]
        ClassHoursSameStarting,
        /// <summary>
        /// 多个课时有相同的开始时间（日期+时间）
        /// </summary>
        [Description("多个课时有相同的开始时间（日期+时间）")]
        ClassHoursSameStartingTime,
        /// <summary>
        /// 多个课时有相同的开始课位（时间）
        /// </summary>
        [Description("多个课时有相同的开始课位（时间）")]
        ClassHoursSameStartingHour,
        /// <summary>
        /// 多个课时间最小间隔天数
        /// </summary>
        [Description("多个课时间最小间隔天数")]
        MinDaysBetweenClassHours,
        /// <summary>
        /// 单个课时有多个优先开始时间
        /// </summary>
        [Description("单个课时有多个优先开始时间")]
        ClassHourRequiredStartingTimes,
        /// <summary>
        /// 单个课时有多个优先课位
        /// </summary>
        [Description("单个课时有多个优先课位")]
        ClassHourRequiredTimes,
        /// <summary>
        /// 多个课时间最大间隔天数
        /// </summary>
        [Description("多个课时间最大间隔天数")]
        MaxDaysBetweenClassHours,
        /// <summary>
        /// 多个课时有多个优先开始时间
        /// </summary>
        [Description("多个课时有多个优先开始时间")]
        ClassHoursRequiredStartingTimes,
        /// <summary>
        /// 多个课时有多个优先课位
        /// </summary>
        [Description("多个课时有多个优先课位")]
        ClassHoursRequiredTimes,
        /// <summary>
        /// 多个课时不同时开课
        /// </summary>
        [Description("多个课时不同时开课")]
        ClassHoursNotOverlap,
        /// <summary>
        /// 单个课时有一个优先开始时间
        /// </summary>
        [Description("单个课时有一个优先开始时间")]
        ClassHourRequiredStartingTime,
        /// <summary>
        /// 多个课时在选定课位中同时开课最大数量
        /// </summary>
        [Description("多个课时在选定课位中同时开课最大数量")]
        ClassHoursMaxConcurrencyInSelectedTime,
        /// <summary>
        /// 多个课时在选定课位中占用的最大数量
        /// </summary>
        [Description("多个课时在选定课位中占用的最大数量")]
        ClassHoursOccupyMaxTimeFromSelection,
        /// <summary>
        /// 对2个课时设置连排
        /// </summary>
        [Description("对2个课时设置连排")]
        TwoClassHoursContinuous,
        /// <summary>
        /// 对2个课时排序
        /// </summary>
        [Description("对2个课时排序")]
        TwoClassHoursOrdered,
        /// <summary>
        /// 给2个课时分组
        /// </summary>
        [Description("给2个课时分组")]
        TwoClassHoursGrouped,
        /// <summary>
        /// 给3个课时分组
        /// </summary>
        [Description("给3个课时分组")]
        ThreeClassHoursGrouped,
        /// <summary>
        /// 教师的不可用时间
        /// </summary>
        [Description("教师的不可用时间")]
        TeacherNotAvailableTimes,
        /// <summary>
        /// 教师每周最大工作天数
        /// </summary>
        [Description("教师每周最大工作天数")]
        TeacherMaxDaysPerWeek,
        /// <summary>
        /// 所有教师每周最大工作天数
        /// </summary>
        [Description("所有教师每周最大工作天数")]
        TeachersMaxDaysPerWeek,
        /// <summary>
        /// 教师每天最大课程间隔
        /// </summary>
        [Description("教师每天最大课程间隔")]
        TeacherMaxGapsPerDay,
        /// <summary>
        /// 所有教师每天最大课程间隔
        /// </summary>
        [Description("所有教师每天最大课程间隔")]
        TeachersMaxGapsPerDay,
        /// <summary>
        /// 所有教师每天最大课时数
        /// </summary>
        [Description("所有教师每天最大课时数")]
        TeachersMaxHoursDaily,
        /// <summary>
        /// 教师每天最大课时数
        /// </summary>
        [Description("教师每天最大课时数")]
        TeacherMaxHoursDaily
    }
}
