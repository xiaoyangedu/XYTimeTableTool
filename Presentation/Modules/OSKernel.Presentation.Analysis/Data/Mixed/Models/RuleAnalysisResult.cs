namespace OSKernel.Presentation.Analysis.Data.Mixed.Models
{
    public class RuleAnalysisResult : BaseModel
    {
        public string RuleName { get; set; }

        public bool HasConflict { get; set; } = false;

        public string HasConflictString
        {
            get
            {
                return HasConflict == true ? "是" : "否";
            }
        }
    }
}
