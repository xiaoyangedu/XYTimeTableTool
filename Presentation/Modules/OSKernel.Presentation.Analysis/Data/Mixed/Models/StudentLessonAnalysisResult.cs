namespace OSKernel.Presentation.Analysis.Data.Mixed.Models
{
    public class StudentLessonAnalysisResult:BaseModel
    {
        public string StudentID { get; set; }

        public string StudentName { get; set; }

        public int Lesson { get; set; }

        public int RedundantLesson { get; set; }
    }
}
