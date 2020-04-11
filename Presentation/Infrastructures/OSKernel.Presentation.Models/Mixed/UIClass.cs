using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Mixed
{
    /// <summary>
    /// UI班级
    /// </summary>
    public class UIClass : ObservableObject, ICloneable
    {
        public int NO { get; set; }

        private bool _isChecked;

        private string _name;

        private int _capacity;

        private string _teacherString;

        private bool _hasOperation;

        public string ID { get; set; }

        public string CourseID { get; set; }

        public string LevelID { get; set; }

        public List<string> TeacherIDs { get; set; }

        public List<string> StudentIDs { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        /// <summary>
        /// 科目
        /// </summary>
        public string Course { get; set; }

        public string Level { get; set; }

        /// <summary>
        /// 显示
        /// </summary>
        string _display;
        public string Display
        {
            get
            {
                if (string.IsNullOrEmpty(Level))
                {
                    if (string.IsNullOrEmpty(ID))
                    {
                        return string.Empty;
                    }
                    else
                        return $"{Course}-{Name}";
                }
                else
                {
                    return $"{Course}-{Level}-{Name}";
                }
            }
            set
            {
                _display = value;
            }
        }

        /// <summary>
        /// 层名称
        /// </summary>
        public string LevelName
        {
            get
            {
                if (string.IsNullOrEmpty(Level))
                {
                    return Name;
                }
                else
                {
                    return $"{Level}-{Name}";
                }
            }
        }

        /// <summary>
        /// 班额
        /// </summary>
        public int Capacity
        {
            get
            {
                return _capacity;
            }

            set
            {
                _capacity = value;
                RaisePropertyChanged(() => Capacity);
            }
        }

        /// <summary>
        /// 教室字符串
        /// </summary>
        public string TeacherString
        {
            get
            {
                return _teacherString;
            }

            set
            {
                _teacherString = value;
                RaisePropertyChanged(() => TeacherString);
            }
        }

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
        /// 课时索引（用于同时开课使用）
        /// </summary>
        public List<UIClassHourIndex> HourIndexs { get; set; }

        public bool HasOperation
        {
            get
            {
                return _hasOperation;
            }

            set
            {
                _hasOperation = value;
                RaisePropertyChanged(() => HasOperation);
            }
        }

        public UIClass()
        {
            this.TeacherIDs = new List<string>();
            this.StudentIDs = new List<string>();
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
    }
}
