using System.Collections.Generic;
using XYKernel.OS.Common.Enums;

namespace XYKernel.OS.Common.Models.Administrative
{
    /// <summary>
    /// 行政班
    /// </summary>
    public class CPCase
    {
        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否自动
        /// </summary>
        public bool IsAuto { get; set; }


        #region 基础信息

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 模型版本
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// 学段
        /// </summary>
        public LearningPeriod LearningPeriod { get; set; } = LearningPeriod.SeniorMiddleSchool;

        /// <summary>
        /// 教师任教多个班级的，同一课时是否需要尽量集中在半天内完成教学
        /// </summary>
        public bool IsTeacherHalfDay { get; set; }

        /// <summary>
        /// 任教2个班级且多课时的学科是否需要尽量考虑每天上课的先后顺序
        /// </summary>
        public bool IsTeacherClassBalance { get; set; }

        /// <summary>
        /// 每天上午第一节、最后一节和下午第一节课是否需要尽量保证每个老师的均衡性
        /// </summary>
        public bool IsTeacherPositionBalance { get; set; }

        /// <summary>
        /// 体育课是否可以排课每天的最后一节（考虑天黑不利于教学）
        /// </summary>
        public bool IsPEAllowLast { get; set; }

        /// <summary>
        /// 教师所任教的两个班的连排课是否要排在同一个半天（同天）
        /// </summary>
        public bool IsTeacherContinousSameDay { get; set; }

        /// <summary>
        /// 教师所任教的两个班的课程间隔是否尽量不能超过1-2节课
        /// </summary>
        public bool IsTeacherDayGapsLimit { get; set; }

        /// <summary>
        /// 如果学科课时为2课时，是否2课时需要间隔1天
        /// </summary>
        public bool IsTwoClassHourLimit { get; set; }

        /// <summary>
        /// 如果学科课时为3课时，是否3课时中至少有2课时间隔1天
        /// </summary>
        public bool IsThreeClassHourLimit { get; set; }

        /// <summary>
        /// 主要科目的教案齐头是否需要尽量满足在同天内
        /// </summary>
        public bool IsMajorCourseSameDay { get; set; }

        #endregion


        /// <summary>
        /// 课位
        /// </summary>
        public List<CoursePositionModel> Positions { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public List<ClassHourModel> ClassHours { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public List<ClassModel> Classes { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public List<CourseModel> Courses { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        public List<TeacherModel> Teachers { get; set; }

        public CPCase()
        {
            ClassHours = new List<ClassHourModel>();
            Classes = new List<ClassModel>();
            Courses = new List<CourseModel>();
            Teachers = new List<TeacherModel>();
            Positions = new List<CoursePositionModel>();
        }
    }
}
