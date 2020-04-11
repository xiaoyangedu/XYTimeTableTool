using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Mixed
{
    /// <summary>
    /// 课程
    /// </summary>
    public class UICourse : ObservableObject
    {
        private bool _isChecked;
        private string _colorString;

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
        /// 课程ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 层集合
        /// </summary>
        public ObservableCollection<UILevel> Levels { get; set; }

        /// <summary>
        /// 默认层
        /// </summary>
        public UILevel DefaultLevel { get; set; }

        /// <summary>
        /// 颜色字符串
        /// </summary>
        public string ColorString
        {
            get
            {
                return _colorString;
            }

            set
            {
                _colorString = value;
                RaisePropertyChanged(() => ColorString);
            }
        }

        public UICourse()
        {
            this.Levels = new ObservableCollection<UILevel>();
        }
    }
}
