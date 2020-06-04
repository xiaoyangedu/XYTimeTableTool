using OSKernel.Presentation.Analysis.Data.Administrative.Models;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace OSKernel.Presentation.Analysis.Data.Administrative.Views
{
    public class TeacherViewModel : CommonViewModel, IInitilize
    {
        private List<TeacherClassAnalysisResult> _teacherClasses;

        public List<TeacherClassAnalysisResult> TeacherClasses
        {
            get
            {
                return _teacherClasses;
            }

            set
            {
                _teacherClasses = value;
                RaisePropertyChanged(() => TeacherClasses);
            }
        }

        public TeacherViewModel()
        {
            this.TeacherClasses = new List<TeacherClassAnalysisResult>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            var rule = CommonDataManager.GetAminRule(base.LocalID);

            this.TeacherClasses = DataAnalysis.GetTeacherClassAnalysisResult(cp, rule);
        }
    }
}
