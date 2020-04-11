using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Rule
{
    public class UIAlgoBase : GalaSoft.MvvmLight.ObservableObject
    {
        private bool isChecked;

        private int weight = 100;

        private bool _isActive;

        public UIAlgoBase()
        {
        }

        /// <summary>
        /// 选中
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }

            set
            {
                isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int NO { get; set; }

        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                _isActive = value;
                RaisePropertyChanged(() => IsActive);
            }
        }

        public int Weight
        {
            get
            {
                return weight;
            }

            set
            {
                weight = value;
                RaisePropertyChanged(() => Weight);
            }
        }
    }
}
