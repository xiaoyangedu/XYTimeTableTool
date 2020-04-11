using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Mixed
{
    /// <summary>
    /// 课时索引绑定类
    /// </summary>
    public class UIClassHourIndex : GalaSoft.MvvmLight.ObservableObject
    {
        private bool isChecked;

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 选中状态
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
    }
}
