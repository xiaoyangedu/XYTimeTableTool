using GalaSoft.MvvmLight;
using OSKernel.Presentation.Models.Administrative;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Base
{
    /// <summary>
    /// 课程设置
    /// </summary>
    public class UIClass : ObservableObject, ICloneable
    {
        private bool isChecked;

        private bool hasOperation;

        private string teacherString;

        public string ID { get; set; }

        public string CourseID { get; set; }

        public string Course { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 科目+班级
        /// </summary>
        public string Display
        {
            get
            {
                return $"{Course}-{Name}";
            }
        }

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

        /// <summary>
        /// 课时索引（用于同时开课使用）
        /// </summary>
        public List<UIClassHourIndex> HourIndexs { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public ObservableCollection<UIClassCourse> Courses { get; set; }

        /// <summary>
        /// 教师字符串
        /// </summary>
        public string TeacherString
        {
            get
            {
                return teacherString;
            }

            set
            {
                teacherString = value;
                RaisePropertyChanged(() => TeacherString);
            }
        }

        public UIClass()
        {
            this.Courses = new ObservableCollection<UIClassCourse>();
            this.HourIndexs = new List<UIClassHourIndex>();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public UIClass DeepClone()
        {
            UIClass classModel = this.Clone() as UIClass;
            classModel.HourIndexs = this.HourIndexs.Select(s =>
            {
                return new UIClassHourIndex()
                {
                    Index = s.Index,
                    IsChecked = s.IsChecked
                };

            })?.ToList();

            return classModel;
        }

        public void RaiseChanged()
        {
            this.RaisePropertyChanged(() => TeacherString);
        }
    }
}
