using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views.Model
{
    /// <summary>
    /// 页面逻辑学生志愿选择
    /// </summary>
    public class UIStudentPreselection
    {
        /// <summary>
        /// 学生ID
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// 学生名称
        /// </summary>
        public string Student { get; set; }

        /// <summary>
        /// 学生选择志愿详细
        /// </summary>
        public List<UIPreselectionDetail> Details { get; set; }

        public UIStudentPreselection()
        {
            this.Details = new List<UIPreselectionDetail>();
        }
    }

    /// <summary>
    /// 学生选择详细
    /// </summary>
    public class UIPreselectionDetail : GalaSoft.MvvmLight.ObservableObject
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
                return $"{Course}-{Level}";
            }
        }
    }
}
