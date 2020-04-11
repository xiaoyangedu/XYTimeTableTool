using GalaSoft.MvvmLight;
using OSKernel.Presentation.Core.DataManager;
using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Mixed.AlgoRule;
using XYKernel.OS.Common.Models.Mixed.Rule;

namespace OSKernel.Presentation.Core.ViewModel
{
    public class CommonViewModel : ViewModelBase
    {
        private string _comments;

        private bool _showLoading;

        private Dictionary<string, WeightTypeEnum> _weights;

        private WeightTypeEnum _selectWeight;

        /// <summary>
        /// 显示Loading
        /// </summary>
        public bool ShowLoading
        {
            get
            {
                return _showLoading;
            }

            set
            {
                _showLoading = value;
                RaisePropertyChanged(() => ShowLoading);
            }
        }

        public Dictionary<string, WeightTypeEnum> Weights
        {
            get
            {
                return _weights;
            }

            set
            {
                _weights = value;
                RaisePropertyChanged(() => Weights);
            }
        }

        /// <summary>
        /// 选择权重
        /// </summary>
        public WeightTypeEnum SelectWeight
        {
            get
            {
                return _selectWeight;
            }

            set
            {
                _selectWeight = value;
                RaisePropertyChanged(() => SelectWeight);
                BatchSetWeight(_selectWeight);
            }
        }

        /// <summary>
        /// 界面备注信息
        /// </summary>
        public string Comments
        {
            get
            {
                return _comments;
            }

            set
            {
                _comments = value;
                RaisePropertyChanged(() => Comments);
            }
        }

        [Dependency]
        public ICommonDataManager CommonDataManager { get; set; }

        [Dependency]
        public IResultDataManager ResultDataManager { get; set; }

        [Dependency]
        public IPatternDataManager PatternDataManager { get; set; }

        /// <summary>
        /// 当前方案ID
        /// </summary>
        public string LocalID
        {
            get
            {
                return CommonDataManager?.LocalID;
            }
        }

        #region 走班模式

        public CLCase GetClCase(string localID)
        {
            var caseModel = CommonDataManager.GetLocalCase(localID);

            if (caseModel.Pattern == Models.Enums.PatternTypeEnum.None)
            {
                return CommonDataManager.GetCLCase(localID);
            }
            else
            {
                return PatternDataManager.GetCase(localID);
            }
        }

        /// <summary>
        /// 获取规则
        /// </summary>
        /// <param name="localID"></param>
        /// <returns></returns>
        public Rule GetClRule(string localID)
        {
            var caseModel = CommonDataManager.GetLocalCase(localID);

            if (caseModel.Pattern == Models.Enums.PatternTypeEnum.None)
            {
                return CommonDataManager.GetMixedRule(localID);
            }
            else
            {
                return PatternDataManager.GetRule(localID);
            }
        }

        /// <summary>
        /// 获取算法规则
        /// </summary>
        /// <param name="localID"></param>
        /// <returns></returns>
        public AlgoRule GetCLAlgoRule(string localID)
        {
            var caseModel = CommonDataManager.GetLocalCase(localID);

            if (caseModel.Pattern == Models.Enums.PatternTypeEnum.None)
            {
                return CommonDataManager.GetMixedAlgoRule(localID);
            }
            else
            {
                return PatternDataManager.GetAlgoRule(localID);
            }
        }

        public void Serialize(CLCase cl, string localID)
        {
            var caseModel = CommonDataManager.GetLocalCase(localID);

            if (caseModel.Pattern == Models.Enums.PatternTypeEnum.None)
            {
                cl.Serialize(localID);
            }
            else
            {
                cl.SerializePatternCase(localID);
            }
        }

        public void SerializePatternRule(Rule rule, string localID)
        {
            var caseModel = CommonDataManager.GetLocalCase(localID);

            if (caseModel.Pattern == Models.Enums.PatternTypeEnum.None)
            {
                rule.Serialize(localID);
            }
            else
            {
                rule.SerializePatternRule(localID);
            }
        }

        public void SerializePatternAlgo(AlgoRule algo, string localID)
        {
            var caseModel = CommonDataManager.GetLocalCase(localID);

            if (caseModel.Pattern == Models.Enums.PatternTypeEnum.None)
            {
                algo.Serialize(localID);
            }
            else
            {
                algo.SerializePatternAlgo(localID);
            }
        }

        #endregion

        /// <summary>
        /// 批量设置权重
        /// </summary>
        /// <param name="weightEnum"></param>
        public virtual void BatchSetWeight(WeightTypeEnum weightEnum)
        {

        }
    }
}
