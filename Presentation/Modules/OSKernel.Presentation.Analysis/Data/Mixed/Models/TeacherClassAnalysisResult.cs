using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Analysis.Data.Mixed.Models
{
    public class TeacherClassAnalysisResult : BaseModel
    {
        public TeacherModel Teacher { get; set; }

        public string ClassID { get; set; }

        public string ClassName { get; set; }

        public string CourseID { get; set; }

        public string CourseName { get; set; }

        public string LevelID { get; set; }

        public string LevelName { get; set; }

        public int LevelLesson { get; set; }

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
                if (string.IsNullOrEmpty(LevelName))
                {
                    return $"{CourseName}-{ClassName}";
                }
                else
                {
                    return $"{CourseName}-{LevelName}-{ClassName}";
                }
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
                //return $"{TeacherLesson}/{LevelLesson}";
            }
        }

        #endregion
    }
}
