using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Base
{
    /// <summary>
    /// 学生信息
    /// </summary>
    public class UIStudent : ObservableObject
    {
        private bool _isChecked;

        /// <summary>
        /// 序号
        /// </summary>
        public int NO { get; set; }

        /// <summary>
        /// 学生ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 学生名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 学生选择志愿详细
        /// </summary>
        public List<UIPreselection> Preselections { get; set; }

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
        /// 扩展对象
        /// </summary>
        public IDictionary<string, object> ExpandoObject
        {
            get
            {
                return _expandoObject;
            }

            set
            {
                _expandoObject = value;
                RaisePropertyChanged(() => ExpandoObject);
            }
        }

        private IDictionary<string, object> _expandoObject;

        public UIStudent()
        {
            this.Preselections = new List<UIPreselection>();
        }
    }
}
