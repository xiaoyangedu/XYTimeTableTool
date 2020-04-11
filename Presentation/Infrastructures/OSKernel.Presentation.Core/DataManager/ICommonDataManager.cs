using OSKernel.Presentation.Models;
using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Administrative;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Core.DataManager
{
    /// <summary>
    /// 基础数据缓存
    /// </summary>
    public interface ICommonDataManager
    {
        Dictionary<string, CPCase> CPCases { get; set; }

        Dictionary<string, CLCase> CLCases { get; set; }

        Dictionary<string, XYKernel.OS.Common.Models.Administrative.Rule.Rule> AdminRules { get; set; }

        Dictionary<string, XYKernel.OS.Common.Models.Mixed.Rule.Rule> MixedRules { get; set; }

        Dictionary<string, XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule> AdminAlgoes { get; set; }

        Dictionary<string, XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule> MixedAlgoes { get; set; }

        /// <summary>
        /// 行政班高级规则备注
        /// </summary>
        Dictionary<AdministrativeAlgoRuleEnum, string> AdministrativeAlgoComments { get; set; }

        /// <summary>
        /// 行政班规则备注
        /// </summary>
        Dictionary<AdministrativeRuleEnum, string> AdministrativeRuleComments { get; set; }

        /// <summary>
        /// 走班高级规则备注
        /// </summary>
        Dictionary<MixedAlgoRuleEnum, string> MixedAlgoComments { get; set; }

        /// <summary>
        /// 走班规则备注
        /// </summary>
        Dictionary<MixedRuleEnum, string> MixedRuleComments { get; set; }

        /// <summary>
        /// 当前方案ID
        /// </summary>
        string LocalID { get; set; }

        /// <summary>
        /// 本地方案
        /// </summary>
        IList<Case> LocalCases { get; set; }

        XYKernel.OS.Common.Models.Administrative.Rule.Rule GetAminRule(string localID);

        XYKernel.OS.Common.Models.Mixed.Rule.Rule GetMixedRule(string localID);

        XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule GetAminAlgoRule(string localID);

        XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule GetMixedAlgoRule(string localID);

        void AddAdminRule(string localID, XYKernel.OS.Common.Models.Administrative.Rule.Rule model);

        void AddMixedRule(string localID, XYKernel.OS.Common.Models.Mixed.Rule.Rule model);

        void AddAminAlgoRule(string localID, XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule model);

        void AddMixedAlgoRule(string localID, XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule model);

        void AddAdminCase(string localID, CPCase model);

        void AddMixedCase(string localID, CLCase model);

        CLCase GetCLCase(string localID);

        CPCase GetCPCase(string localID);

        Case GetLocalCase(string localID);

        void RemoveFullCase(string localID);


        string GetAdminAlgoComments(AdministrativeAlgoRuleEnum algoEnum);
        string GetAdminRuleComments(AdministrativeRuleEnum algoEnum);
        string GetMixedAlgoComments(MixedAlgoRuleEnum algoEnum);
        string GetMixedRuleComments(MixedRuleEnum algoEnum);

    }
}
