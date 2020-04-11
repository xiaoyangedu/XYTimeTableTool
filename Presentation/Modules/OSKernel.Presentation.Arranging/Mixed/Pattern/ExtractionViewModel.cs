using OSKernel.Presentation.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern
{
    public class ExtractionViewModel : GalaSoft.MvvmLight.ObservableObject
    {
        private int _extractionRate = 10;

        private int _classCapacity = 1;

        public ExtractionViewModel()
        {

        }

        /// <summary>
        /// 精度
        /// </summary>
        public int ExtractionRate
        {
            get
            {
                return _extractionRate;
            }

            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;

                _extractionRate = value;
                RaisePropertyChanged(() => ExtractionRate);
            }
        }

        /// <summary>
        /// 班额
        /// </summary>
        public int ClassCapacity
        {
            get
            {
                return _classCapacity;
            }

            set
            {
                if (value < 0)
                    value = 0;

                _classCapacity = value;
                RaisePropertyChanged(() => ClassCapacity);
            }
        }
    }
}
