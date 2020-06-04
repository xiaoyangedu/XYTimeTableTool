namespace OSKernel.Presentation.Analysis.Data.Mixed.Models
{
    public class ClassAnalysisResult : BaseModel
    {
        public string ClassID { get; set; }

        public string ClassName { get; set; }

        public string CourseID { get; set; }

        public string CourseName { get; set; }

        public string LevelID { get; set; }

        public string LevelName { get; set; }

        public int Capacity { get; set; }

        /// <summary>
        /// 平均班额
        /// </summary>
        public int AveCapacity { get; set; }

        /// <summary>
        /// 冗余班额
        /// </summary>
        public int RedundantCapacity { get; set; }

        #region UI

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

        #endregion
    }
}
