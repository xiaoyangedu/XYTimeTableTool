using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Models.Result
{
    /// <summary>
    /// 调整结果周
    /// </summary>
    public class UIAdjustResultWeek : GalaSoft.MvvmLight.ObservableObject
    {
        private DayPeriodModel _period;

        private ContentControl _monday;

        private ContentControl _tuesday;

        private ContentControl _wednesday;

        private ContentControl _thursday;

        private ContentControl _friday;

        private ContentControl _saturday;

        private ContentControl _sunday;

        /// <summary>
        /// 周期字符串
        /// </summary>
        public DayPeriodModel Period
        {
            get
            {
                return _period;
            }

            set
            {
                _period = value;
                RaisePropertyChanged(() => Period);
            }
        }

        /// <summary>
        /// 课位类型
        /// </summary>
        public XYKernel.OS.Common.Enums.Position PositionType { get; set; }

        public ContentControl Monday
        {
            get
            {
                return _monday;
            }

            set
            {
                _monday = value;
                RaisePropertyChanged(() => Monday);
            }
        }
        public ContentControl Tuesday
        {
            get
            {
                return _tuesday;
            }

            set
            {
                _tuesday = value;
                RaisePropertyChanged(() => Tuesday);
            }
        }
        public ContentControl Wednesday
        {
            get
            {
                return _wednesday;
            }

            set
            {
                _wednesday = value;
                RaisePropertyChanged(() => Wednesday);
            }
        }
        public ContentControl Thursday
        {
            get
            {
                return _thursday;
            }

            set
            {
                _thursday = value;
                RaisePropertyChanged(() => Thursday);
            }
        }
        public ContentControl Friday
        {
            get
            {
                return _friday;
            }

            set
            {
                _friday = value;
                RaisePropertyChanged(() => Friday);
            }
        }
        public ContentControl Saturday
        {
            get
            {
                return _saturday;
            }

            set
            {
                _saturday = value;
                RaisePropertyChanged(() => Saturday);
            }
        }
        public ContentControl Sunday
        {
            get
            {
                return _sunday;
            }

            set
            {
                _sunday = value;
                RaisePropertyChanged(() => Sunday);
            }
        }

        public UIAdjustResultWeek()
        {
            this.Monday = new ContentControl();
            this.Tuesday = new ContentControl();
            this.Wednesday = new ContentControl();
            this.Thursday = new ContentControl();
            this.Friday = new ContentControl();
            this.Saturday = new ContentControl();
            this.Sunday = new ContentControl();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
