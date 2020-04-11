using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Base
{
    /// <summary>
    /// 课位类
    /// </summary>
    public class UIPosition : ObservableObject, ICloneable
    {
        private string _periodString;

        private bool _isMondayChecked = true;

        private bool _isTuesdayChecked = true;

        private bool _isWednesdayChecked = true;

        private bool _isThursdayChecked = true;

        private bool _isFridayChecked = true;

        private bool _isSaturdayChecked;

        private bool _isSundayChecked;

        private bool _canOperation;

        public bool IsMondayChecked
        {
            get
            {
                return _isMondayChecked;
            }

            set
            {
                _isMondayChecked = value;
                RaisePropertyChanged(() => IsMondayChecked);
            }
        }

        public bool IsTuesdayChecked
        {
            get
            {
                return _isTuesdayChecked;
            }

            set
            {
                _isTuesdayChecked = value;
                RaisePropertyChanged(() => IsTuesdayChecked);
            }
        }

        public bool IsWednesdayChecked
        {
            get
            {
                return _isWednesdayChecked;
            }

            set
            {
                _isWednesdayChecked = value;
                RaisePropertyChanged(() => IsWednesdayChecked);
            }
        }

        public bool IsThursdayChecked
        {
            get
            {
                return _isThursdayChecked;
            }

            set
            {
                _isThursdayChecked = value;
                RaisePropertyChanged(() => IsThursdayChecked);
            }
        }

        public bool IsFridayChecked
        {
            get
            {
                return _isFridayChecked;
            }

            set
            {
                _isFridayChecked = value;
                RaisePropertyChanged(() => IsFridayChecked);
            }
        }

        public bool IsSaturdayChecked
        {
            get
            {
                return _isSaturdayChecked;
            }

            set
            {
                _isSaturdayChecked = value;
                RaisePropertyChanged(() => IsSaturdayChecked);
            }
        }

        public bool IsSundayChecked
        {
            get
            {
                return _isSundayChecked;
            }

            set
            {
                _isSundayChecked = value;
                RaisePropertyChanged(() => IsSundayChecked);
            }
        }

        /// <summary>
        /// 是否可以操作（可以操作界面会显示操作按钮）
        /// </summary>
        public bool CanOperation
        {
            get
            {
                return _canOperation;
            }

            set
            {
                _canOperation = value;
                RaisePropertyChanged(() => CanOperation);
            }
        }

        /// <summary>
        /// 周期字符串
        /// </summary>
        public string PeriodString
        {
            get
            {
                return _periodString;
            }

            set
            {
                _periodString = value;
                RaisePropertyChanged(() => PeriodString);
            }
        }

        /// <summary>
        /// 课位类型
        /// </summary>
        public XYKernel.OS.Common.Enums.Position PositionType { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public UIPosition Copy()
        {
            UIPosition position = this.MemberwiseClone() as UIPosition;

            return position;
        }
    }
}
