using System.Collections.Generic;
using System.Text;

namespace OSKernel.Presentation.Analysis.Data.Mixed.Models
{
    public class RuleAnalysisDetailResult : BaseModel
    {
        public string RuleName { get; set; }

        public List<string> DetailInfo { get; set; }

        #region UI

        public string Description
        {
            get
            {
                if (DetailInfo?.Count == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return DetailInfo[0];
                }
            }
        }

        /// <summary>
        /// 是否显示详细按钮(目前只显示一行错误信息,如果信息超过两行则该属性为True)
        /// </summary>
        public bool ShowDetails
        {
            get
            {
                if (DetailInfo?.Count > 1)
                    return true;
                else
                    return false;
            }
            set
            { }
        }

        #endregion
    }
}
