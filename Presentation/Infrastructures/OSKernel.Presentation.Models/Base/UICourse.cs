using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Base
{
    /// <summary>
    /// 课程
    /// </summary>
    public class UICourse : ObservableObject, ICloneable
    {
        private bool _isChecked;
        private string _name;
        private string _colorString;
        private int _lessons;
        private bool _isEnable;

        public string ID { get; set; }

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

        #region 集中设置课时使用

        /// <summary>
        /// 课时
        /// </summary>
        public List<UIClassHourCount> ClassHours { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
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

        #endregion

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

        public bool IsEnable
        {
            get
            {
                return _isEnable;
            }

            set
            {
                _isEnable = value;
                RaisePropertyChanged(() => IsEnable);
            }
        }

        #endregion

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public UICourse GetInstance()
        {
            return this.Clone() as UICourse;
        }
    }
}
