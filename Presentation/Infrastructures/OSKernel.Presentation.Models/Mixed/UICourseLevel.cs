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
    /// 课程层
    /// </summary>
    public class UICourseLevel : ObservableObject
    {
        private string _levelID;
        private string _courseID;
        private string _level;
        private string _course;
        private int _lessons;
        private bool _isChecked;
        private int _studentCount;

        private ObservableCollection<UIClass> _classes;

        public string LevelID
        {
            get
            {
                return _levelID;
            }

            set
            {
                _levelID = value;
                RaisePropertyChanged(() => LevelID);
            }
        }

        public string CourseID
        {
            get
            {
                return _courseID;
            }

            set
            {
                _courseID = value;
                RaisePropertyChanged(() => CourseID);
            }
        }

        public string Level
        {
            get
            {
                return _level;
            }

            set
            {
                _level = value;
                RaisePropertyChanged(() => Level);
            }
        }

        public string Course
        {
            get
            {
                return _course;
            }

            set
            {
                _course = value;
                RaisePropertyChanged(() => Course);
            }
        }

        public int Lessons
        {
            get
            {
                return _lessons;
            }

            set
            {
                _lessons = value;
                RaisePropertyChanged(() => Lessons);
            }
        }

        /// <summary>
        /// 显示字符串
        /// </summary>
        public string Display
        {
            get
            {
                if (string.IsNullOrEmpty(Level))
                {
                    return Course;
                }
                else
                {
                    return $"{Course}-{Level}";
                }
            }
        }

        /// <summary>
        /// 显示集中设置班额
        /// </summary>
        public bool ShowUniformCapacity
        {
            get
            {
                if (this.Classes?.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public ObservableCollection<UIClass> Classes
        {
            get
            {
                return _classes;
            }

            set
            {
                _classes = value;
                RaisePropertyChanged(() => Classes);
            }
        }

        /// <summary>
        /// 是否选中
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

        public int StudentCount
        {
            get
            {
                return _studentCount;
            }

            set
            {
                _studentCount = value;
                RaisePropertyChanged(() => StudentCount);
            }
        }

        #region 选中数量

        private int _selectClasses;

        /// <summary>
        /// 选中班级
        /// </summary>
        public int SelectClasses
        {
            get
            {
                return _selectClasses;
            }

            set
            {
                _selectClasses = value;
                RaisePropertyChanged(() => SelectClasses);
                RaisePropertyChanged(() => ShowSelectClassCountArea);
            }
        }

        public bool ShowSelectClassCountArea
        {
            get
            {
                if (SelectClasses == 0)
                    return false;
                else
                    return true;
            }
        }

        #endregion

        public UICourseLevel()
        {
            this.Classes = new ObservableCollection<UIClass>();
        }

        /// <summary>
        /// 刷新改变
        /// </summary>
        public void RaiseChanged()
        {
            this.RaisePropertyChanged(() => ShowUniformCapacity);
        }
    }
}
