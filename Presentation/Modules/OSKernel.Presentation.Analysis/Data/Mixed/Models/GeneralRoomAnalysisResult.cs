namespace OSKernel.Presentation.Analysis.Data.Mixed.Models
{
    public class GeneralRoomAnalysisResult
    {
        public int CaseAvailableRoom { get; set; }

        public int CaseMinimizedRoom { get; set; }

        public int FreeRoomNumber { get; set; }

        public string CaseAvailableRoomString
        {
            get
            {
                if (CaseAvailableRoom == 0)
                    return "不限";
                else
                    return CaseAvailableRoom.ToString();
            }
            set
            {

            }
        }
    }
}
