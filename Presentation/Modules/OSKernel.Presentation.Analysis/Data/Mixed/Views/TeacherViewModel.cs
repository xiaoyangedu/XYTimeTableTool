using OSKernel.Presentation.Analysis.Data.Mixed.Models;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace OSKernel.Presentation.Analysis.Data.Mixed.Views
{
    public class TeacherViewModel : CommonViewModel, IInitilize
    {
        private List<TeacherClassAnalysisResult> _teacherAnalysis;

        public List<TeacherClassAnalysisResult> TeacherAnalysis
        {
            get
            {
                return _teacherAnalysis;
            }

            set
            {
                _teacherAnalysis = value;
                RaisePropertyChanged(() => TeacherAnalysis);
            }
        }

        public TeacherViewModel()
        {
            this.TeacherAnalysis = new List<TeacherClassAnalysisResult>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = base.GetClCase(base.LocalID);

            var rule = base.GetClRule(base.LocalID);

            this.TeacherAnalysis = DataAnalysis.GetTeacherClassAnalysisResult(cl, rule);
        }
    }
}
