using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Base
{
    /// <summary>
    /// 学生志愿
    /// </summary>
    public class UIPreselection : GalaSoft.MvvmLight.ObservableObject
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
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 层ID
        /// </summary>
        public string LevelID { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string Course { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 显示名称(课程+层)
        /// </summary>
        public string Display
        {
            get
            {
                if (LevelID.Equals("0"))
                {
                    return $"{Course}";
                }
                else
                {
                    return $"{Course}-{Level}";
                }
            }
        }

    }
}
