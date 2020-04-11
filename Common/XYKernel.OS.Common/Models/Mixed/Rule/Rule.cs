using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Mixed.Rule
{
    /// <summary>
    /// 规则
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// 教师时间
        /// </summary>
        public List<TeacherTimeRule> TeacherTimes { get; set; }

        /// <summary>
        /// 同时开课限制
        /// </summary>
        public List<CourseLimitRule> CourseLimits { get; set; }

        /// <summary>
        /// 课时分散
        /// </summary>
        public List<ClassHourAverageRule> ClassHourAverages { get; set; }

        /// <summary>
        /// 同时开课
        /// </summary>
        public List<ClassHourSameOpenRule> ClassHourSameOpens { get; set; }

        /// <summary>
        /// 上下午课时
        /// </summary>
        public List<AmPmClassHourRule> AmPmClassHours { get; set; }

        /// <summary>
        /// 课程连排
        /// </summary>
        public List<ArrangeContinousRule> ArrangeContinuous { get; set; }

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
        /// 课程排课时间
        /// </summary>
        public List<CourseTimeRule> CourseTimes { get; set; }

        public Rule()
        {
            this.AmPmClassHours = new List<AmPmClassHourRule>();
            this.ArrangeContinuous = new List<ArrangeContinousRule>();
            this.ClassHourAverages = new List<ClassHourAverageRule>();
            this.ClassHourSameOpens = new List<ClassHourSameOpenRule>();
            this.CourseLimits = new List<CourseLimitRule>();
            this.CourseTimes = new List<CourseTimeRule>();
            this.MaxDaysPerWeek = new List<TeacherMaxDaysPerWeekRule>();
            this.MaxGapsPerDay = new List<TeacherMaxGapsPerDayRule>();
            this.MaxHoursDaily = new List<TeacherMaxHoursDailyRule>();
            this.TeacherTimes = new List<TeacherTimeRule>();
        }
    }
}
