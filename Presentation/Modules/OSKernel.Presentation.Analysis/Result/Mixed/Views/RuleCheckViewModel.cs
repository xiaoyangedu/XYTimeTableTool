using OSKernel.Presentation.Analysis.Dialogs;
using OSKernel.Presentation.Analysis.Result.Models;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.Http;
using OSKernel.Presentation.Core.Http.Table;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models.Mixed.Result;

namespace OSKernel.Presentation.Analysis.Result.Mixed.Views
{
    public class RuleCheckViewModel : CommonViewModel, IInitilize
    {
        private string _errorMessage;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            set
            {
                _errorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
            }
        }

        private List<RuleAnalysisDetailResult> _rules;

        /// <summary>
        /// 规则
        /// </summary>
        public List<RuleAnalysisDetailResult> Rules
        {
            get
            {
                return _rules;
            }

            set
            {
                _rules = value;
                RaisePropertyChanged(() => Rules);
            }
        }

        /// <summary>
        /// 错误详细信息
        /// </summary>
        public ICommand ErroDetailsCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<RuleAnalysisDetailResult>(erroDetailsCommand);
            }
        }

        public RuleCheckViewModel()
        {
            this.Rules = new List<RuleAnalysisDetailResult>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var result = ResultDataManager.CurrentResult;

            var localResult = base.LocalID.DeSerializeLocalResult<ResultModel>(result.TaskID);
            if (localResult == null)
            {
                var value = WebAPI.Instance.GetMixedResult(result.TaskID);
                if (value.Item1)
                {
                    localResult = value.Item2;
                }
                else
                {
                    this.ErrorMessage = $"*{value.Item3}";
                    return;
                }
            }

            var cl = CommonDataManager.GetCLCase(base.LocalID);
            var rule = CommonDataManager.GetMixedRule(base.LocalID);
            var algo = CommonDataManager.GetMixedAlgoRule(base.LocalID);
            var rules = ResultAnalysis.GetResultRuleFitDegreeAnalysis(cl, rule, algo, localResult);

            int no = 0;
            rules?.ForEach(r =>
            {
                r.NO = ++no;
            });

            this.Rules = rules;
        }

        void erroDetailsCommand(RuleAnalysisDetailResult ruleAnalysis)
        {
            ErroDetailsWindow window = new ErroDetailsWindow(ruleAnalysis.DetailInfo);
            window.ShowDialog();
        }
    }
}
