using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Models.Result
{
    public class UIResultWeek : GalaSoft.MvvmLight.ObservableObject
    {
        private DayPeriodModel _period;

        private ObservableCollection<ContentControl> _mondays;

        private ObservableCollection<ContentControl> _tuesdays;

        private ObservableCollection<ContentControl> _wednesdays;

        private ObservableCollection<ContentControl> _thursdays;

        private ObservableCollection<ContentControl> _fridays;

        private ObservableCollection<ContentControl> _saturdays;

        private ObservableCollection<ContentControl> _sundays;

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

        public ObservableCollection<ContentControl> Mondays
        {
            get
            {
                return _mondays;
            }

            set
            {
                _mondays = value;
                RaisePropertyChanged(() => Mondays);
            }
        }
        public ObservableCollection<ContentControl> Tuesdays
        {
            get
            {
                return _tuesdays;
            }

            set
            {
                _tuesdays = value;
                RaisePropertyChanged(() => Tuesdays);
            }
        }
        public ObservableCollection<ContentControl> Wednesdays
        {
            get
            {
                return _wednesdays;
            }

            set
            {
                _wednesdays = value;
                RaisePropertyChanged(() => Wednesdays);
            }
        }
        public ObservableCollection<ContentControl> Thursdays
        {
            get
            {
                return _thursdays;
            }

            set
            {
                _thursdays = value;
                RaisePropertyChanged(() => Thursdays);
            }
        }
        public ObservableCollection<ContentControl> Fridays
        {
            get
            {
                return _fridays;
            }

            set
            {
                _fridays = value;
                RaisePropertyChanged(() => Fridays);
            }
        }
        public ObservableCollection<ContentControl> Saturdays
        {
            get
            {
                return _saturdays;
            }

            set
            {
                _saturdays = value;
                RaisePropertyChanged(() => Saturdays);
            }
        }
        public ObservableCollection<ContentControl> Sundays
        {
            get
            {
                return _sundays;
            }

            set
            {
                _sundays = value;
                RaisePropertyChanged(() => Sundays);
            }
        }

        public UIResultWeek()
        {
            this.Mondays = new ObservableCollection<ContentControl>();
            this.Tuesdays = new ObservableCollection<ContentControl>();
            this.Wednesdays = new ObservableCollection<ContentControl>();
            this.Thursdays = new ObservableCollection<ContentControl>();
            this.Fridays = new ObservableCollection<ContentControl>();
            this.Saturdays = new ObservableCollection<ContentControl>();
            this.Sundays = new ObservableCollection<ContentControl>();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
