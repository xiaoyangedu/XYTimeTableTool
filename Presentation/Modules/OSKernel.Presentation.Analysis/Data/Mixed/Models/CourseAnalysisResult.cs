namespace OSKernel.Presentation.Analysis.Data.Mixed.Models
{
    public class CourseAnalysisResult : BaseModel
    {
        public string CourseID { get; set; }
        public string LevelID { get; set; }
        public string CourseName { get; set; }
        public string LevelName { get; set; }
        public int StudentNumber { get; set; }
        public int ClassNumber { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string CourseDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(LevelName))
                {
                    return $"{CourseName}-{LevelName}";
                }
                else
                {
                    return CourseName;
                }
            }
        }
    }
}
