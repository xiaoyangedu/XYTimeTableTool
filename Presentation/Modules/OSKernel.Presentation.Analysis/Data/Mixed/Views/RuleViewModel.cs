using OSKernel.Presentation.Analysis.Data.Mixed.Models;
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

namespace OSKernel.Presentation.Analysis.Data.Mixed.Views
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
                return new GalaSoft.MvvmLight.Command.RelayCommand<RuleAnalysisDetailResult>(ErroDetails);
            }
        }

        public RuleViewModel()
        {
            this.Rules = new List<RuleAnalysisDetailResult>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = base.GetClCase(base.LocalID);
            var rule = base.GetClRule(base.LocalID);
            var rules = DataAnalysis.GetRuleAnalysisDetailResult(cl, rule);

            int no = 0;
            rules.ForEach(r =>
            {
                r.NO = ++no;
            });

            this.Rules = rules;
        }

        void ErroDetails(RuleAnalysisDetailResult ruleAnalysis)
        {
            ErroDetailsWindow detailsWindow = new ErroDetailsWindow(ruleAnalysis.DetailInfo);
            detailsWindow.ShowDialog();
        }
    }
}
