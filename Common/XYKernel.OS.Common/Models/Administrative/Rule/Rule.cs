using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 规则
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// 课程互斥
        /// </summary>
        public List<MutexGroupRule> Mutexes { get; set; }

        /// <summary>
        /// 合班上课
        /// </summary>
        public List<ClassUnionRule> ClassUnions { get; set; }

        /// <summary>
        /// 教师排课时间
        /// </summary>
        public List<TeacherTimeRule> TeacherTimes { get; set; }

        /// <summary>
        /// 同时开课限制
        /// </summary>
        public List<CourseLimitRule> CourseLimits { get; set; }

        /// <summary>
        /// 师徒跟随
        /// </summary>
        public List<MasterApprenticeRule> MasterApprenttices { get; set; }

        /// <summary>
        /// 教案齐头
        /// </summary>
        public List<TeachingPlanFlushRule> PlanFlushes { get; set; }

        /// <summary>
        /// 课时分散
        /// </summary>
        public List<ClassHourAverageRule> ClassHourAverages { get; set; }

        /// <summary>
        /// 同时开课
        /// </summary>
        public List<ClassHourSameOpenRule> ClassHourSameOpens { get; set; }

        /// <summary>
        /// 教师上下午不连排
        /// </summary>
        public List<TeacherAmPmNoContinousRule> AmPmNoContinues { get; set; }

        /// <summary>
        /// 教师每天最大课时间隔
        /// </summary>
        public List<TeacherMaxGapsPerDayRule> MaxGapsPerDay { get; set; }

        /// <summary>
        /// 教师每周最大工作天数
        /// </summary>
        public List<TeacherMaxDaysPerWeekRule> MaxDaysPerWeek { get; set; }

        /// <summary>
        /// 教师每天最大课时数
        /// </summary>
        public List<TeacherMaxHoursDailyRule> MaxHoursDaily { get; set; }

        /// <summary>
        /// 连排齐头
        /// </summary>
        public List<ArrangeContinousPlanRule> ContinuousPlanFlushes { get; set; }

        /// <summary>
        /// 课程排课时间
        /// </summary>
        public List<CourseTimeRule> CourseTimes { get; set; }

        /// <summary>
        /// 课程连排
        /// </summary>
        public List<CourseArrangeContinousRule> CourseArranges { get; set; }

        /// <summary>
        /// 单双周
        /// </summary>
        public List<OddDualWeekRule> OddDualWeeks { get; set; }

        /// <summary>
        /// 上下午课时
        /// </summary>
        public List<AmPmClassHourRule> AmPmClassHours { get; set; }

        /// <summary>
        /// 教案平头
        /// </summary>
        public List<TeacherPriorityBalanceRule> ClassHourPriorityBalance { get; set; }

        /// <summary>
        /// 辅修科目规避时间
        /// </summary>
        public List<MinorCourseAvoidTimeRule> MinorCourseAvoidTime { get; set; }

        /// <summary>
        /// 主修科目优先时间及排课占比
        /// </summary>
        public List<MajorCoursePreferredTimeRule> MajorCoursePreferredTime { get; set; }

        /// <summary>
        /// 教师在特定课位的课时限制
        /// </summary>
        public List<TeacherLimitInSpecialTimeRule> LimitInSpecialTime { get; set; }

        /// <summary>
        /// 各班级第一个课时尽量不重复
        /// </summary>
        public AvoidRepeatInClassFirstSlotRule AvoidRepeatInClassFirstSlot { get; set; }

        /// <summary>
        /// 教师半天上课
        /// </summary>
        public List<TeacherHalfDayWorkRule> HalfDayWork { get; set; }

        /// <summary>
        /// 课表锁定
        /// </summary>
        public TimeTableLockRule TimeTableLockedTimes { get; set; }

        public Rule()
        {
            this.Mutexes = new List<MutexGroupRule>();
            this.ClassUnions = new List<ClassUnionRule>();
            this.TeacherTimes = new List<TeacherTimeRule>();
            this.CourseLimits = new List<CourseLimitRule>();
            this.MasterApprenttices = new List<MasterApprenticeRule>();
            this.PlanFlushes = new List<TeachingPlanFlushRule>();
            this.ClassHourAverages = new List<ClassHourAverageRule>();
            this.ClassHourSameOpens = new List<ClassHourSameOpenRule>();
            this.AmPmNoContinues = new List<TeacherAmPmNoContinousRule>();
            this.MaxGapsPerDay = new List<TeacherMaxGapsPerDayRule>();
            this.MaxDaysPerWeek = new List<TeacherMaxDaysPerWeekRule>();
            this.MaxHoursDaily = new List<TeacherMaxHoursDailyRule>();
            this.ContinuousPlanFlushes = new List<ArrangeContinousPlanRule>();
            this.CourseTimes = new List<CourseTimeRule>();
            this.CourseArranges = new List<CourseArrangeContinousRule>();
            this.OddDualWeeks = new List<OddDualWeekRule>();
            this.AmPmClassHours = new List<AmPmClassHourRule>();
            this.ClassHourPriorityBalance = new List<TeacherPriorityBalanceRule>();
            this.MinorCourseAvoidTime = new List<MinorCourseAvoidTimeRule>();
            this.MajorCoursePreferredTime = new List<MajorCoursePreferredTimeRule>();
            this.LimitInSpecialTime = new List<TeacherLimitInSpecialTimeRule>();
            this.AvoidRepeatInClassFirstSlot = new AvoidRepeatInClassFirstSlotRule();
            this.HalfDayWork = new List<TeacherHalfDayWorkRule>();
            this.TimeTableLockedTimes = new TimeTableLockRule();
        }
    }
}
