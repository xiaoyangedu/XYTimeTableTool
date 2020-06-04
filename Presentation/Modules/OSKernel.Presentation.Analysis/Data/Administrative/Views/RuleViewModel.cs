using OSKernel.Presentation.Analysis.Data.Administrative.Models;
using OSKernel.Presentation.Analysis.Dialogs;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Analysis.Data.Administrative.Views
{
    public class RuleViewModel : CommonViewModel, IInitilize
    {
        private List<RuleAnalysisDetailResult> _rules;

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

        public ICommand ErroDetailsCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<RuleAnalysisDetailResult>(erroDetailsCommand);
            }
        }

        public RuleViewModel()
        {
            this.Rules = new List<RuleAnalysisDetailResult>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            var rule = CommonDataManager.GetAminRule(base.LocalID);
            var rules = DataAnalysis.GetRuleAnalysisDetailResult(cp, rule);

            int no = 0;
            rules.ForEach(r =>
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
