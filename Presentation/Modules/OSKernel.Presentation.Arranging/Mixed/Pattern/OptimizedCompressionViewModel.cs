using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using XYKernel.OS.Common.Models.Mixed.Result;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern
{
    public class OptimizedCompressionViewModel : CommonViewModel, IInitilize
    {
        private UIResult _selectResult;

        private List<UIResult> _results;

        /// <summary>
        /// 结果
        /// </summary>
        public List<UIResult> Results
        {
            get
            {
                return _results;
            }

            set
            {
                _results = value;
                RaisePropertyChanged(() => Results);
            }
        }

        /// <summary>
        /// 选中结果
        /// </summary>
        public UIResult SelectResult
        {
            get
            {
                return _selectResult;
            }

            set
            {
                _selectResult = value;
                RaisePropertyChanged(() => SelectResult);
            }
        }

        public OptimizedCompressionViewModel()
        {

        }

        [InjectionMethod]
        public void Initilize()
        {
            this.Results = ResultDataManager.GetResults(base.LocalID);
        }
    }
}
