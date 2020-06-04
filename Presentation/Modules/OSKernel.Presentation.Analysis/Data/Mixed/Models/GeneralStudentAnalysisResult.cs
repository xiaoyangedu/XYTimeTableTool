namespace OSKernel.Presentation.Analysis.Data.Mixed.Models
{
    public class GeneralStudentAnalysisResult:BaseModel
    {
        public int Lesson { get; set; }

        public int StudentNumber { get; set; }

        public int FreeSlotNumber { get; set; }
    }
}
