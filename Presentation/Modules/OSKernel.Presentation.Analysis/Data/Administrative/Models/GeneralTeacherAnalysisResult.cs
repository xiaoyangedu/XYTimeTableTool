using XYKernel.OS.Common.Models.Administrative;

namespace OSKernel.Presentation.Analysis.Data.Administrative.Models
{
    public class GeneralTeacherAnalysisResult: BaseModel
    {
        public TeacherModel Teacher { get; set; }

        public int Lesson { get; set; }

        public int ClassNumber { get; set; }

        public float LessonRatio { get; set; }
    }
}
