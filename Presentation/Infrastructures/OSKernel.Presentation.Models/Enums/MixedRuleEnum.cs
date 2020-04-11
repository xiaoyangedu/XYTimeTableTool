using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 走班枚举
    /// </summary>
    public enum MixedRuleEnum
    {
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
        /// 可用教室数
        /// </summary>
        [Description("可用教室数")]
        AvailableRoom,
    }
}
