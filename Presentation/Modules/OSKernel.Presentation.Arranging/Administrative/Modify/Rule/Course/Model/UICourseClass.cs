using OSKernel.Presentation.Models.Administrative;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model
{
    public class UICourseClass : GalaSoft.MvvmLight.ObservableObject
    {
        private bool isChecked;

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

        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public List<UIClass> Classes { get; set; }

        private bool hasOperation;

        /// <summary>
        /// 是否有操作
        /// </summary>
        public bool HasOperation
        {
            get
            {
                return hasOperation;
            }

            set
            {
                hasOperation = value;
                RaisePropertyChanged(() => HasOperation);
            }
        }

        public UICourseClass()
        {
            this.Classes = new List<UIClass>();
        }
    }
}
