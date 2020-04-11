using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Models.Time
{
    public class UITwoStatusWeek : GalaSoft.MvvmLight.ObservableObject
    {
        private DayPeriodModel _period;

        private UIWeek _monday;

        private UIWeek _tuesday;

        private UIWeek _wednesday;

        private UIWeek _thursday;

        private UIWeek _friday;

        private UIWeek _saturday;

        private UIWeek _sunday;

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

        public UIWeek Monday
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

        public UIWeek Tuesday
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

        public UIWeek Wednesday
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

        public UIWeek Thursday
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

        public UIWeek Friday
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

        public UIWeek Saturday
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

        public UIWeek Sunday
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

        public UITwoStatusWeek()
        {
            this.Monday = new UIWeek();
            this.Tuesday = new UIWeek();
            this.Wednesday = new UIWeek();
            this.Thursday = new UIWeek();
            this.Friday = new UIWeek();
            this.Saturday = new UIWeek();
            this.Sunday = new UIWeek();
        }

        public void ClearStatus()
        {
            this.Monday = new UIWeek();
            this.Tuesday = new UIWeek();
            this.Wednesday = new UIWeek();
            this.Thursday = new UIWeek();
            this.Friday = new UIWeek();
            this.Saturday = new UIWeek();
            this.Sunday = new UIWeek();
        }

        public void SetStatus(bool status)
        {
            this.Monday.IsChecked = status;
            this.Tuesday.IsChecked = status;
            this.Wednesday.IsChecked = status;
            this.Thursday.IsChecked = status;
            this.Friday.IsChecked = status;
            this.Saturday.IsChecked = status;
            this.Sunday.IsChecked = status;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
