using OSKernel.Presentation.Analysis.Data.Mixed.Models;
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
    public class StudentViewModel : CommonViewModel, IInitilize
    {
        private List<StudentLessonAnalysisResult> studentLessonAnalysis;

        private List<StudentSelectionCombinationAnalysisResult> _studentCombinations;

        public List<StudentLessonAnalysisResult> StudentLessonAnalysis
        {
            get
            {
                return studentLessonAnalysis;
            }

            set
            {
                studentLessonAnalysis = value;
                RaisePropertyChanged(() => StudentLessonAnalysis);
            }
        }

        public List<StudentSelectionCombinationAnalysisResult> StudentCombinations
        {
            get
            {
                return _studentCombinations;
            }

            set
            {
                _studentCombinations = value;
                RaisePropertyChanged(() => StudentCombinations);
            }
        }

        public StudentViewModel()
        {
            this.StudentCombinations = new List<StudentSelectionCombinationAnalysisResult>();
            this.StudentLessonAnalysis = new List<StudentLessonAnalysisResult>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = base.GetClCase(base.LocalID);
            var rule = base.GetClRule(base.LocalID);

            this.studentLessonAnalysis = DataAnalysis.GetStudentLessonAnalysisResult(cl);
            this.StudentCombinations = DataAnalysis.GetStudentSelectionCombinationAnalysisResult(cl);
        }

     
    }
}
