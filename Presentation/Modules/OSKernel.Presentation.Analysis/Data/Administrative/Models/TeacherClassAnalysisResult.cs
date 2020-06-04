using XYKernel.OS.Common.Models.Administrative;

namespace OSKernel.Presentation.Analysis.Data.Administrative.Models
{
    public class TeacherClassAnalysisResult : BaseModel
    {
        public TeacherModel Teacher { get; set; }

        public string ClassID { get; set; }

        public string ClassName { get; set; }

        public string CourseID { get; set; }

        public string CourseName { get; set; }

        public int ClassCourseLesson { get; set; }

        public int TeacherLesson { get; set; }

        /// <summary>
        /// 连排
        /// </summary>
        public int Continous { get; set; }

        #region UI

        /// <summary>
        /// 班级显示
        /// </summary>
        public string ClassDisplay
        {
            get
            {
                return $"{CourseName}-{ClassName}";
            }
        }

        /// <summary>
        /// 课时字段
        /// </summary>
        public string HourDisplay
        {
            get
            {
                return $"{TeacherLesson}";
            }
        }

        #endregion
    }
}
