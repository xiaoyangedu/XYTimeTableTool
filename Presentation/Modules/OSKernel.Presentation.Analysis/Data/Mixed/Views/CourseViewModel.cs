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
    /// <summary>
    /// 课程设置
    /// </summary>
    public class CourseViewModel : CommonViewModel, IInitilize
    {
        List<CourseAnalysisResult> _courseAnalysis;

        /// <summary>
        /// 课程
        /// </summary>
        public List<CourseAnalysisResult> CourseAnalysis
        {
            get
            {
                return _courseAnalysis;
            }

            set
            {
                _courseAnalysis = value;
                RaisePropertyChanged(() => CourseAnalysis);
            }
        }

        public CourseViewModel()
        {
            this.CourseAnalysis = new List<CourseAnalysisResult>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = base.GetClCase(base.LocalID);
            this.CourseAnalysis = DataAnalysis.GetCourseAnalysisResult(cl);
        }
    }
}
