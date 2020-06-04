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
    public class ClassViewModel : CommonViewModel, IInitilize
    {
        private List<ClassAnalysisResult> _classAnalysis;

        public List<ClassAnalysisResult> ClassAnalysis
        {
            get
            {
                return _classAnalysis;
            }

            set
            {
                _classAnalysis = value;
                RaisePropertyChanged(() => ClassAnalysis);
            }
        }

        public ClassViewModel()
        {
            this.ClassAnalysis = new List<ClassAnalysisResult>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = base.GetClCase(base.LocalID);
            this.ClassAnalysis = DataAnalysis.GetClassAnalysisResult(cl);
        }
    }
}
