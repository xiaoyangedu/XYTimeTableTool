using OSKernel.Presentation.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern
{
    public class CompressionViewModel : GalaSoft.MvvmLight.ObservableObject
    {
        private int _compression = 2;

        public CompressionViewModel()
        {

        }

        public int Compression
        {
            get
            {
                return _compression;
            }

            set
            {
                if (value < 2)
                    value = 2;

                _compression = value;
                RaisePropertyChanged(() => Compression);
            }
        }
    }
}
