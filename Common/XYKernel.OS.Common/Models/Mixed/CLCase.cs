using System.Collections.Generic;
using XYKernel.OS.Common.Enums;

namespace XYKernel.OS.Common.Models.Mixed
{
    /// <summary>
    /// 走班
    /// </summary>
    public class CLCase
    {
        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 可用教室
        /// </summary>
        public int RoomLimit { get; set; }

        /// <summary>
        /// 模型版本
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 学段
        /// </summary>
        public LearningPeriod LearningPeriod { get; set; } = LearningPeriod.SeniorMiddleSchool;

        /// <summary>
        /// 是否自动
        /// </summary>
        public bool IsAuto { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<TagModel> Tags { get; set; }

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

        /// <summary>
        /// 学生
        /// </summary>
        public List<StudentModel> Students { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public List<ClassHourModel> ClassHours { get; set; }

        /// <summary>
        /// 课位
        /// </summary>
        public List<CoursePositionModel> Positions { get; set; }

        public CLCase()
        {
            this.Tags = new List<TagModel>();
            this.Classes = new List<ClassModel>();
            this.Courses = new List<CourseModel>();
            this.Teachers = new List<TeacherModel>();
            this.Students = new List<StudentModel>();
            this.ClassHours = new List<ClassHourModel>();
            this.Positions = new List<CoursePositionModel>();
        }
    }
}
