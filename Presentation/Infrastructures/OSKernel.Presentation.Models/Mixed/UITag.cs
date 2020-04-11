using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Mixed
{
    /// <summary>
    /// UI标签模型
    /// </summary>
    public class UITag : ObservableObject
    {
        private bool _isChecked;

        /// <summary>
        /// 选中
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                _isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }

        public string ID { get; set; }

        public string Name { get; set; }
    }
}
