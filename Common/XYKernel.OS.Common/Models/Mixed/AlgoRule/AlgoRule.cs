using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    public class AlgoRule
    {
        /// <summary>
        /// 多个课时有相同的开始日期（日期）
        /// </summary>
        public List<ClassHoursSameStartingDayRule> ClassHoursSameStartingDays { get; set; }

        /// <summary>
        /// 多个课时有相同的开始时间（日期+时间）
        /// </summary>
        public List<ClassHoursSameStartingTimeRule> ClassHoursSameStartingTimes { get; set; }

        /// <summary>
        /// 多个课时有相同的开始课位（时间）
        /// </summary>
        public List<ClassHoursSameStartingHourRule> ClassHoursSameStartingHours { get; set; }

        /// <summary>
        /// 学生不可用时间
        /// </summary>
        //public List<StudentsSetNotAvailableTimesRule> StudentsSetNotAvailableTimes { get; set; }

        /// <summary>
        /// 学生最大天数
        /// </summary>
        //public List<StudentsMaxDaysPerWeekRule> StudentsMaxDaysPerWeeks { get; set; }

        /// <summary>
        /// 教师的不可用时间
        /// </summary>
        public List<TeacherNotAvailableTimesRule> TeacherNotAvailableTimes { get; set; }

        /// <summary>
        /// 教师每周最大工作天数
        /// </summary>
        public List<TeacherMaxDaysPerWeekRule> TeacherMaxDaysPerWeeks { get; set; }

        /// <summary>
        /// 所有教师每周最大工作天数
        /// </summary>
        public List<TeachersMaxDaysPerWeekRule> TeachersMaxDaysPerWeeks { get; set; }

        /// <summary>
        /// 教师每天最大课程间隔
        /// </summary>
        public List<TeacherMaxGapsPerDayRule> TeacherMaxGapsPerDays { get; set; }

        /// <summary>
        /// 所有教师每天最大课程间隔
        /// </summary>
        public List<TeachersMaxGapsPerDayRule> TeachersMaxGapsPerDays { get; set; }

        /// <summary>
        /// 所有教师每天最大课时数
        /// </summary>
        public List<TeachersMaxHoursDailyRule> TeachersMaxHoursDailys { get; set; }

        /// <summary>
        /// 教师每天最大课时数
        /// </summary>
        public List<TeacherMaxHoursDailyRule> TeacherMaxHoursDailys { get; set; }

        /// <summary>
        /// 对2个课时设置连排
        /// </summary>
        public List<TwoClassHoursContinuousRule> TwoClassHoursContinuous { get; set; }

        /// <summary>
        /// 对2个课时排序
        /// </summary>
        public List<TwoClassHoursOrderedRule> TwoClassHoursOrdered { get; set; }

        /// <summary>
        /// 给2个课时分组
        /// </summary>
        public List<TwoClassHoursGroupedRule> TwoClassHoursGrouped { get; set; }

        /// <summary>
        /// 给3个课时分组
        /// </summary>
        public List<ThreeClassHoursGroupedRule> ThreeClassHoursGrouped { get; set; }

        ///// <summary>
        ///// 科目教室单项规则
        ///// </summary>
        //public List<SubjectRequiredRoomRule> SubjectRequiredRooms { get; set; }

        ///// <summary>
        ///// 课时标签教室
        ///// </summary>
        //public List<ClassHourTagRequiredRoomRule> ClassHourTagRequiredRooms { get; set; }

        ///// <summary>
        ///// 科目教室多个规则
        ///// </summary>
        //public List<SubjectRequiredRoomsRule> SubjectRequiredRoomes { get; set; }

        ///// <summary>
        ///// 教室最大课时
        ///// </summary>
        //public List<RoomMaxUsedTimesRule> RoomMaxUsedTimesInfos { get; set; }

        ///// <summary>
        ///// 教室不可用时间
        ///// </summary>
        //public List<RoomNotAvailableTimesRule> RoomNotAvailableTimes { get; set; }

        /// <summary>
        /// 课时最大不同教室
        /// </summary>
        //public List<ClassHoursUseMaxDifferentRoomsRule> ClassHoursUseMaxDifferentRooms { get; set; }

        /// <summary>
        /// 单个课时教室
        /// </summary>
        //public List<ClassHourRequiredRoomRule> ClassHourRequiredRooms { get; set; }

        /// <summary>
        /// 单个课时多个教室
        /// </summary>
        //public List<ClassHourRequiredRoomsRule> ClassHourRequiredRoomes { get; set; }

        /// <summary>
        /// 多个课时教室
        /// </summary>
        //public List<ClassHourTagRequiredRoomsRule> ClassHourTagRequiredRoomes { get; set; }

        /// <summary>
        /// 多个课时间最小课程间隔天数
        /// </summary>
        public List<MinDaysBetweenClassHoursRule> MinDaysBetweenClassHours { get; set; }

        /// <summary>
        /// 多个课时之间的最大间隔天数
        /// </summary>
        public List<MaxDaysBetweenClassHoursRule> MaxDaysBetweenClassHours { get; set; }

        /// <summary>
        /// 单个课时有多个优先开始时间
        /// </summary>
        public List<ClassHourRequiredStartingTimesRule> ClassHourRequiredStartingTimes { get; set; }

        /// <summary>
        /// 多个课时有多个优先开始时间
        /// </summary>
        public List<ClassHoursRequiredStartingTimesRule> ClassHoursRequiredStartingTimes { get; set; }

        /// <summary>
        /// 单个课时有一个优先开始时间
        /// </summary>
        public List<ClassHourRequiredStartingTimeRule> ClassHourRequiredStartingTime { get; set; }

        /// <summary>
        /// 单个课时有多个优先课位
        /// </summary>
        public List<ClassHourRequiredTimesRule> ClassHourRequiredTimes { get; set; }

        /// <summary>
        /// 多个课时有多个优先课位
        /// </summary>
        public List<ClassHoursRequiredTimesRule> ClassHoursRequiredTimes { get; set; }

        /// <summary>
        /// 不同时开课
        /// </summary>
        public List<ClassHoursNotOverlapRule> ClassHoursNotOverlaps { get; set; }

        /// <summary>
        /// 在选定课位中设置多个课时的最大同时开课数量
        /// </summary>
        public List<ClassHoursMaxConcurrencyInSelectedTimeRule> ClassHoursMaxConcurrencyInSelectedTimes { get; set; }

        /// <summary>
        /// 多个课时在选定课位中占用的最大数量
        /// </summary>
        public List<ClassHoursOccupyMaxTimeFromSelectionRule> ClassHoursOccupyMaxTimeFromSelections { get; set; }

        public AlgoRule()
        {
            this.ClassHourRequiredStartingTime = new List<ClassHourRequiredStartingTimeRule>();
            this.ClassHourRequiredStartingTimes = new List<ClassHourRequiredStartingTimesRule>();
            this.ClassHourRequiredTimes = new List<ClassHourRequiredTimesRule>();
            this.ClassHoursMaxConcurrencyInSelectedTimes = new List<ClassHoursMaxConcurrencyInSelectedTimeRule>();
            this.ClassHoursNotOverlaps = new List<ClassHoursNotOverlapRule>();
            this.ClassHoursOccupyMaxTimeFromSelections = new List<ClassHoursOccupyMaxTimeFromSelectionRule>();
            this.ClassHoursRequiredStartingTimes = new List<ClassHoursRequiredStartingTimesRule>();
            this.ClassHoursRequiredTimes = new List<ClassHoursRequiredTimesRule>();
            this.ClassHoursSameStartingDays = new List<ClassHoursSameStartingDayRule>();
            this.ClassHoursSameStartingHours = new List<ClassHoursSameStartingHourRule>();
            this.ClassHoursSameStartingTimes = new List<ClassHoursSameStartingTimeRule>();
            this.MaxDaysBetweenClassHours = new List<MaxDaysBetweenClassHoursRule>();
            this.MinDaysBetweenClassHours = new List<MinDaysBetweenClassHoursRule>();
            this.TeacherMaxDaysPerWeeks = new List<TeacherMaxDaysPerWeekRule>();
            this.TeacherMaxGapsPerDays = new List<TeacherMaxGapsPerDayRule>();
            this.TeacherMaxHoursDailys = new List<TeacherMaxHoursDailyRule>();
            this.TeacherNotAvailableTimes = new List<TeacherNotAvailableTimesRule>();
            this.TeachersMaxDaysPerWeeks = new List<TeachersMaxDaysPerWeekRule>();
            this.TeachersMaxGapsPerDays = new List<TeachersMaxGapsPerDayRule>();
            this.TeachersMaxHoursDailys = new List<TeachersMaxHoursDailyRule>();
            this.ThreeClassHoursGrouped = new List<ThreeClassHoursGroupedRule>();
            this.TwoClassHoursContinuous = new List<TwoClassHoursContinuousRule>();
            this.TwoClassHoursGrouped = new List<TwoClassHoursGroupedRule>();
            this.TwoClassHoursOrdered = new List<TwoClassHoursOrderedRule>();
        }
    }
}
