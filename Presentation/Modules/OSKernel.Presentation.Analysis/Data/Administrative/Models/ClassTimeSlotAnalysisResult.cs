namespace OSKernel.Presentation.Analysis.Data.Administrative.Models
{
    public class ClassTimeSlotAnalysisResult: BaseModel
    {
        public string ClassID { get; set; }

        public string ClassName { get; set; }

        public float ClassLesson { get; set; }

        public int CourseNumber { get; set; }

        public float FreeSlotNumber { get; set; }
    }
}
