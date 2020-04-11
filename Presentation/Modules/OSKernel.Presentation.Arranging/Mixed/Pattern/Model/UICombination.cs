using OSKernel.Presentation.Models.Mixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Pattern.Extend;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern.Model
{
    /// <summary>
    /// 组合
    /// </summary>
    public class UICombination : GalaSoft.MvvmLight.ObservableObject
    {
        private bool _isChecked;

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

            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int NO { get; set; }

        /// <summary>
        /// 组合名称
        /// </summary>
        public string Combination { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        public int Students { get; set; }

        /// <summary>
        /// 选中志愿
        /// </summary>
        public List<SelectionModel> Selections { get; set; }

        public UICombination()
        {
            Selections = new List<SelectionModel>();
        }
    }
}
