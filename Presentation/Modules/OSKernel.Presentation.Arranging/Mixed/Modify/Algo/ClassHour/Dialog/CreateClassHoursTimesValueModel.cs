using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.CustomControl.Enums;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Models.Mixed;
using OSKernel.Presentation.Models.Time;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour.Dialog
{
    public class CreateClassHoursTimesValueModel : BaseWindowModel, IInitilize
    {
        private int _maxConcurrencyNumber;

        private int _maxOccupyNumber;

        private MixedAlgoRuleEnum _timeRule;

        private ObservableCollection<UIClassHour> _classHours;

        private ObservableCollection<UIClassHour> _targetHours;

        public ObservableCollection<UIClassHour> ClassHours
        {
            get
            {
                return _classHours;
            }

            set
            {
                _classHours = value;
                RaisePropertyChanged(() => ClassHours);
            }
        }

        public ObservableCollection<UIClassHour> TargetHours
        {
            get
            {
                return _targetHours;
            }

            set
            {
                _targetHours = value;
                RaisePropertyChanged(() => TargetHours);
            }
        }

        public List<UIClassHour> Sources { get; set; }

        /// <summary>
        /// 最大同时开课数
        /// </summary>
        public bool ShowSameMaxNumber
        {
            get
            {
                if (_timeRule == MixedAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 占用最大数量
        /// </summary>
        public bool ShowMaxNumber
        {
            get
            {
                if (_timeRule == MixedAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection)
                    return true;
                else
                    return false;
            }
        }

        private List<UITwoStatusWeek> _periods;

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

        public int MaxConcurrencyNumber
        {
            get
            {
                return _maxConcurrencyNumber;
            }

            set
            {
                if (value < 0)
                    return;

                _maxConcurrencyNumber = value;
                RaisePropertyChanged(() => MaxConcurrencyNumber);
            }
        }

        public ICommand SourceCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(sourceCommand);
            }
        }

        public ICommand TargetCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(targetCommand);
            }
        }

        public ICommand ClearAllListCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(clearAllListCommand);
            }
        }

        public ICommand SelectedAllListCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(selectAllListCommand);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(save);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(cancel);
            }
        }

        /// <summary>
        /// 最大占用数
        /// </summary>
        public int MaxOccupyNumber
        {
            get
            {
                return _maxOccupyNumber;
            }

            set
            {
                _maxOccupyNumber = value;
                RaisePropertyChanged(() => MaxOccupyNumber);
            }
        }

        public CreateClassHoursTimesValueModel()
        {
            ClassHours = new ObservableCollection<UIClassHour>();

            TargetHours = new ObservableCollection<UIClassHour>();
        }

        public void SetValue(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule)
        {
            this.getBase(opratorEnum, timeRule);
        }
        public void SetValue(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule, UIClassHourRule rule)
        {
            this.getBase(opratorEnum, timeRule);

            this.bind(rule);
        }

        void getBase(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule)
        {
            base.OpratorEnum = opratorEnum;
            this._timeRule = timeRule;
            this.TitleString = $"{ opratorEnum.GetLocalDescription()}-{timeRule.GetLocalDescription()}";
            this.RaisePropertyChanged(() => ShowRead);
            this.RaisePropertyChanged(() => ShowEdit);
            this.RaisePropertyChanged(() => ShowMaxNumber);
            this.RaisePropertyChanged(() => ShowSameMaxNumber);

            var cl = base.GetClCase(base.LocalID);
            this.Sources = cl.GetClassHours(cl.ClassHours.Select(ch => ch.ID)?.ToArray());

            var results = new List<UITwoStatusWeek>();
            var groups = cl.Positions.GroupBy(p => p.DayPeriod.Period);
            if (groups != null)
            {
                foreach (var g in groups)
                {
                    var first = g.First();
                    UITwoStatusWeek week = new UITwoStatusWeek()
                    {
                        Period = first.DayPeriod,
                        PositionType = first.Position,
                    };
                    week.SetStatus(true);
                    results.Add(week);
                }
            }
            this.Periods = results;

            this.Search();
        }

        void bind(UIClassHourRule receive)
        {
            this.UID = receive.UID;
            this.Weight = receive.Weight;
            this.IsActive = receive.IsActive;
            this.MaxConcurrencyNumber = receive.MaxConcurrency;
            this.MaxOccupyNumber = receive.MaxOccupy;

            if (receive.ClassHours != null)
            {
                this.TargetHours = new ObservableCollection<UIClassHour>(receive.ClassHours);
            }

            // 先将所有状态更改为False.
            this.Periods.ForEach(p =>
            {
                p.SetStatus(false);
            });

            // 绑定状态
            receive.Periods.ForEach(t =>
            {
                var period = this.Periods.First(p => p.Period.Period == t.Period);
                if (period != null)
                {
                    if (t.Day == DayOfWeek.Monday)
                    {
                        period.Monday.IsChecked = true;
                    }
                    else if (t.Day == DayOfWeek.Tuesday)
                    {
                        period.Tuesday.IsChecked = true;
                    }
                    else if (t.Day == DayOfWeek.Wednesday)
                    {
                        period.Wednesday.IsChecked = true;
                    }
                    else if (t.Day == DayOfWeek.Thursday)
                    {
                        period.Thursday.IsChecked = true;
                    }
                    else if (t.Day == DayOfWeek.Friday)
                    {
                        period.Friday.IsChecked = true;
                    }
                    else if (t.Day == DayOfWeek.Saturday)
                    {
                        period.Saturday.IsChecked = true;
                    }
                    else if (t.Day == DayOfWeek.Sunday)
                    {
                        period.Sunday.IsChecked = true;
                    }
                }
            });
        }

        void sourceCommand(object selectItem)
        {
            var model = selectItem as UIClassHour;
            if (model != null)
                this.AddClassHour(model);
        }

        void targetCommand(object selectItem)
        {
            var model = selectItem as UIClassHour;
            if (model != null)
                this.RemoveClassHour(model);
        }

        void clearAllListCommand()
        {
            this.TargetHours.ToList()?.ForEach((removeItem) =>
            {
                this.RemoveClassHour(removeItem);
            });
        }

        void selectAllListCommand()
        {
            this.ClassHours.ToList()?.ForEach((removeItem) =>
            {
                this.AddClassHour(removeItem);
            });
        }

        void save(object obj)
        {
            CreateClassHoursTimesValue window = obj as CreateClassHoursTimesValue;

            if (!this.Validate())
                return;

            if (this.OpratorEnum == OperatorEnum.Add)
            {
                window.Add = new UIClassHourRule
                {
                    UID = this.UID,
                    IsActive = this.IsActive,
                    MaxConcurrency = this.MaxConcurrencyNumber,
                    MaxOccupy = this.MaxOccupyNumber,
                    ClassHours = this.TargetHours?.ToList(),
                    Periods = this.getTimes()
                };
            }
            else
            {
                window.Modify = new UIClassHourRule
                {
                    UID = this.UID,
                    IsActive = this.IsActive,
                    MaxConcurrency = this.MaxConcurrencyNumber,
                    MaxOccupy = this.MaxOccupyNumber,
                    ClassHours = this.TargetHours?.ToList(),
                    Periods = this.getTimes()
                };
            }

            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            CreateClassHoursTimesValue window = obj as CreateClassHoursTimesValue;
            window.Close();
        }

        public void AddClassHour(UIClassHour classHour)
        {
            if (this.TargetHours.Any(t => t.ID == classHour.ID))
            {
                return;
            }
            else
            {
                this.TargetHours.Add(classHour);
            }
        }

        public void RemoveClassHour(UIClassHour classHour)
        {
            if (this.TargetHours.Any(t => t.ID == classHour.ID))
            {
                this.TargetHours.Remove(classHour);
            }
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <returns></returns>
        List<DayPeriodModel> getTimes()
        {
            List<DayPeriodModel> times = new List<DayPeriodModel>();

            var periods = this.Periods.Where(p =>
            p.PositionType != XYKernel.OS.Common.Enums.Position.AB &&
            p.PositionType != XYKernel.OS.Common.Enums.Position.PB &&
            p.PositionType != XYKernel.OS.Common.Enums.Position.Noon);

            if (periods != null)
            {
                foreach (var p in periods)
                {
                    if (p.Monday.IsChecked)
                    {
                        times.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Monday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Tuesday.IsChecked)
                    {
                        times.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Tuesday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Wednesday.IsChecked)
                    {
                        times.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Wednesday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Thursday.IsChecked)
                    {
                        times.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Thursday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Friday.IsChecked)
                    {
                        times.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Friday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Saturday.IsChecked)
                    {
                        times.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Saturday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Sunday.IsChecked)
                    {
                        times.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Sunday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                }
            }

            return times;
        }

        public override void Search()
        {
            var source = this.Sources?.ToList();

            if (!string.IsNullOrEmpty(base.SelectMixedTeacher?.ID))
            {
                source = source.Where(s => s.Teachers.Any(t => t.ID.Equals(base.SelectMixedTeacher?.ID)))?.ToList();
            }

            if (!string.IsNullOrEmpty(base.SelectMixedCourse?.ID))
            {
                source = source.Where(s => s.CourseID.Equals(base.SelectMixedCourse?.ID))?.ToList();
            }

            if (!string.IsNullOrEmpty(base.SelectMixedClass?.ID))
            {
                source = source.Where(s => s.ClassID.Equals(base.SelectMixedClass?.ID))?.ToList();
            }

            if (!string.IsNullOrEmpty(base.SelectMixedTag?.ID))
            {
                source = source.Where(s =>
                {
                    if (s.Tags == null)
                    {
                        return false;
                    }
                    else
                    {
                        return s.Tags.Contains(base.SelectMixedTag?.ID);
                    }

                })?.ToList();
            }

            if (source != null)
            {
                ClassHours = new ObservableCollection<UIClassHour>(source);
            }
        }

        public void Initilize()
        {

        }

        public bool Validate()
        {
            if (this.TargetHours?.Count == 0)
            {
                this.ShowDialog("提示信息", "选择课时列表为空!", DialogSettingType.NoButton);
                return false;
            }
            return true;
        }
    }
}
