using OSKernel.Presentation.Analysis.Data;
using System.Collections.Generic;

namespace OSKernel.Presentation.Analysis.Result.Models
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
                if ((DetailInfo?.Count ?? 0) == 0)
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

        /// <summary>
        /// 是否满足
        /// </summary>
        public string HasConflictString
        {
            get
            {
                if (DetailInfo == null)
                {
                    return "是";
                }
                else
                {
                    if (DetailInfo.Count == 0)
                        return "是";
                    else
                        return "否";
                }

            }
        }

        #endregion
    }
}
