using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course.Model;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Models.Time;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Enums;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Dialog
{
    public class CourseLimitPositionModel : CommonViewModel, IInitilize
    {
        private List<UITwoStatusWeek> _periods;

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(saveCommand);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(cancelCommand);
            }
        }

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        /// <summary>
        /// 周期
        /// </summary>
        public List<UITwoStatusWeek> Periods
        {
            get
            {
                return _periods;
            }

            set
            {
                _periods = value;
                RaisePropertyChanged(() => Periods);
            }
        }

        public CourseLimitPositionModel()
        {
            this.Periods = new List<UITwoStatusWeek>();

            base.Weights = new Dictionary<string, WeightTypeEnum>()
            {
                { "高", WeightTypeEnum.Hight},
                { "中", WeightTypeEnum.Medium},
                { "低", WeightTypeEnum.Low},
            };

            base.SelectWeight = WeightTypeEnum.Hight;
        }

        [InjectionMethod]
        public void Initilize()
        {
            var rule = base.GetClRule(base.LocalID);

            var cl = base.GetClCase(base.LocalID);

            var groups = cl.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);

            var periods = new List<UITwoStatusWeek>();
            foreach (var g in groups)
            {
                Dictionary<DayOfWeek, bool> weekSelected = new Dictionary<DayOfWeek, bool>();
                g.ToList().ForEach(gg =>
                {
                    weekSelected.Add(gg.DayPeriod.Day, gg.IsSelected);
                });

                var first = g.First();
                var dayPeriod = first.DayPeriod;
                var period = new UITwoStatusWeek()
                {
                    PositionType = first.Position,
                    Monday = new UIWeek
                    {
                        IsChecked = weekSelected[DayOfWeek.Monday]
                    },
                    Tuesday = new UIWeek
                    {
                        IsChecked = weekSelected[DayOfWeek.Tuesday]
                    },
                    Wednesday = new UIWeek
                    {
                        IsChecked = weekSelected[DayOfWeek.Wednesday],
                    },
                    Thursday = new UIWeek
                    {
                        IsChecked = weekSelected[DayOfWeek.Thursday],
                    },
                    Friday = new UIWeek
                    {
                        IsChecked = weekSelected[DayOfWeek.Friday],
                    },
                    Saturday = new UIWeek
                    {
                        IsChecked = weekSelected[DayOfWeek.Saturday],
                    },
                    Sunday = new UIWeek
                    {
                        IsChecked = weekSelected[DayOfWeek.Sunday],
                    },
                    Period = dayPeriod
                };

                periods.Add(period);
            }
            this.Periods = periods;
        }

        void userControlCommand(string parms)
        {
            if (parms.Equals("loaded"))
            {

            }
            else if (parms.Equals("unloaded"))
            {

            }
        }

        void saveCommand(object obj)
        {
            var win = obj as CourseLimitPosition;

            win.PeriodLimits = new List<XYKernel.OS.Common.Models.Mixed.Rule.PeriodLimit>();

            var periods = this.Periods.Where(p => p.PositionType != Position.AB && p.PositionType != Position.PB && p.PositionType != Position.Noon);
            foreach (var p in periods)
            {
                if (!String.IsNullOrEmpty(p.Monday.Value))
                {
                    win.PeriodLimits.Add(new XYKernel.OS.Common.Models.Mixed.Rule.PeriodLimit()
                    {
                        DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                        {
                            Day = DayOfWeek.Monday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        },
                        Limit = Convert.ToInt32(p.Monday.Value)
                    });
                }

                if (!String.IsNullOrEmpty(p.Tuesday.Value))
                {
                    win.PeriodLimits.Add(new XYKernel.OS.Common.Models.Mixed.Rule.PeriodLimit()
                    {
                        DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                        {
                            Day = DayOfWeek.Tuesday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        },
                        Limit = Convert.ToInt32(p.Tuesday.Value)
                    });
                }

                if (!String.IsNullOrEmpty(p.Wednesday.Value))
                {
                    win.PeriodLimits.Add(new XYKernel.OS.Common.Models.Mixed.Rule.PeriodLimit()
                    {
                        DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                        {
                            Day = DayOfWeek.Wednesday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        },
                        Limit = Convert.ToInt32(p.Wednesday.Value)
                    });
                }

                if (!String.IsNullOrEmpty(p.Thursday.Value))
                {
                    win.PeriodLimits.Add(new XYKernel.OS.Common.Models.Mixed.Rule.PeriodLimit()
                    {
                        DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                        {
                            Day = DayOfWeek.Thursday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        },
                        Limit = Convert.ToInt32(p.Thursday.Value)
                    });
                }

                if (!String.IsNullOrEmpty(p.Friday.Value))
                {
                    win.PeriodLimits.Add(new XYKernel.OS.Common.Models.Mixed.Rule.PeriodLimit()
                    {
                        DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                        {
                            Day = DayOfWeek.Friday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        },
                        Limit = Convert.ToInt32(p.Friday.Value)
                    });
                }

                if (!String.IsNullOrEmpty(p.Saturday.Value))
                {
                    win.PeriodLimits.Add(new XYKernel.OS.Common.Models.Mixed.Rule.PeriodLimit()
                    {
                        DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                        {
                            Day = DayOfWeek.Saturday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        },
                        Limit = Convert.ToInt32(p.Saturday.Value)
                    });
                }

                if (!String.IsNullOrEmpty(p.Sunday.Value))
                {
                    win.PeriodLimits.Add(new XYKernel.OS.Common.Models.Mixed.Rule.PeriodLimit()
                    {
                        DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                        {
                            Day = DayOfWeek.Sunday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        },
                        Limit = Convert.ToInt32(p.Sunday.Value)
                    });
                }
            }

            win.WeightType = base.SelectWeight;
            win.DialogResult = true;
        }

        void cancelCommand(object obj)
        {
            var win = obj as CourseLimitPosition;
            win.DialogResult = false;
        }

        public void Bind(UICourseLimit bind)
        {
            bind.PeriodLimits.ForEach(pl =>
            {
                var period = this.Periods.FirstOrDefault(p => p.Period.Period == pl.DayPeriod.Period);
                if (period != null)
                {
                    if (pl.DayPeriod.Day == DayOfWeek.Monday)
                    {
                        period.Monday.Value = pl.Limit.ToString();
                    }
                    else if (pl.DayPeriod.Day == DayOfWeek.Tuesday)
                    {
                        period.Tuesday.Value = pl.Limit.ToString();
                    }
                    else if (pl.DayPeriod.Day == DayOfWeek.Wednesday)
                    {
                        period.Wednesday.Value = pl.Limit.ToString();
                    }
                    else if (pl.DayPeriod.Day == DayOfWeek.Thursday)
                    {
                        period.Thursday.Value = pl.Limit.ToString();
                    }
                    else if (pl.DayPeriod.Day == DayOfWeek.Friday)
                    {
                        period.Friday.Value = pl.Limit.ToString();
                    }
                    else if (pl.DayPeriod.Day == DayOfWeek.Saturday)
                    {
                        period.Saturday.Value = pl.Limit.ToString();
                    }
                    else if (pl.DayPeriod.Day == DayOfWeek.Sunday)
                    {
                        period.Sunday.Value = pl.Limit.ToString();
                    }
                }
            });

            base.SelectWeight = bind.Weight;
        }
    }
}
