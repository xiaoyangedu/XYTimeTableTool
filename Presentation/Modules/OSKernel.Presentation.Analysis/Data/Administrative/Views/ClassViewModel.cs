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
    public class ClassViewModel : CommonViewModel, IInitilize
    {
        private List<ClassCourseAnalysisResult> _classes;

        public ClassViewModel()
        {
            this.Classes = new List<ClassCourseAnalysisResult>();
        }

        public List<ClassCourseAnalysisResult> Classes
        {
            get
            {
                return _classes;
            }

            set
            {
                _classes = value;
                RaisePropertyChanged(() => Classes);
            }
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            this.Classes = DataAnalysis.GetClassCourseAnalysisResult(cp);
        }
    }
}
