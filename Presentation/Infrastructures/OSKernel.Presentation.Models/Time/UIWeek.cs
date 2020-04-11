using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Time
{
    public class UIWeek : GalaSoft.MvvmLight.ObservableObject
    {
        private bool _isChecked;

        private bool _isMouseLeft;

        private string _value;

        /// <summary>
        /// 选中状态
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

                this.IsMouseLeft = false;
            }
        }

        /// <summary>
        /// 左键：启用，右键：禁用
        /// </summary>
        public bool IsMouseLeft
        {
            get
            {
                return _isMouseLeft;
            }

            set
            {
                _isMouseLeft = value;
                RaisePropertyChanged(() => IsMouseLeft);
            }
        }

        /// <summary>
        /// 存储输入输入的数值
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
                RaisePropertyChanged(() => Value);
            }
        }
    }
}
