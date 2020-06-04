namespace OSKernel.Presentation.Analysis.Data.Mixed.Models
{
    public class StudentSelectionCombinationAnalysisResult : BaseModel
    {
        public string CombinationName { get; set; }
        public int StudentNumber { get; set; }
        public int Lesson { get; set; }
        public int RedundantLesson { get; set; }
    }
}
