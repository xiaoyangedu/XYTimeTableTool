using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    /// <summary>
    /// 统计ViewModel
    /// </summary>
    public class StatisticStudentSelectionWindowModel : CommonViewModel, IInitilize
    {
        private List<string> _statistics;
        public List<string> Statistics
        {
            get
            {
                return _statistics;
            }

            set
            {
                _statistics = value;
                RaisePropertyChanged(() => Statistics);
            }
        }

        public StatisticStudentSelectionWindowModel()
        {
            this.Statistics = new List<string>();
        }

        [InjectionMethod]
        public void Initilize()
        {

        }

        public void BindData(List<string> bindDatas)
        {
            this.Statistics = bindDatas;
        }
    }
}
