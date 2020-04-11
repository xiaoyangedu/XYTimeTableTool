using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Models.Time;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher.Dialog
{
    public class CreateTeacherNoAvaliableModel : BaseWindowModel, IInitilize
    {
        private List<UITeacher> _teachers;

        private UITeacher _selectTeacher;

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

        public List<UITeacher> Teachers
        {
            get
            {
                return _teachers;
            }

            set
            {
                _teachers = value;
                RaisePropertyChanged(() => Teachers);
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
        /// 选择教师
        /// </summary>
        public UITeacher SelectTeacher
        {
            get
            {
                return _selectTeacher;
            }

            set
            {
                _selectTeacher = value;
                RaisePropertyChanged(() => SelectTeacher);
            }
        }

        public CreateTeacherNoAvaliableModel()
        {
            this.Teachers = new List<UITeacher>();
            this.Periods = new List<UITwoStatusWeek>();
        }

        void save(object obj)
        {
            CreateTeacherNoAvaliable window = obj as CreateTeacherNoAvaliable;

            if (this.OpratorEnum == OperatorEnum.Add)
            {
                if (this.SelectTeacher == null) return;

                window.Add = new UITeacherRule
                {
                    IsActive = this.IsActive,
                    Name = this.SelectTeacher.Name,
                    TeacherID = this.SelectTeacher.ID,
                    UID = Guid.NewGuid().ToString(),
                    Weight = this.Weight,
                    ForbidTimes = this.getforbidTimes()
                };
            }
            else
            {
                window.Modify = new UITeacherRule
                {
                    IsActive = this.IsActive,
                    Name = this.SelectTeacher.Name,
                    TeacherID = this.SelectTeacher.ID,
                    UID = Guid.NewGuid().ToString(),
                    Weight = this.Weight,
                    ForbidTimes = this.getforbidTimes()
                };
            }

            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            CreateTeacherNoAvaliable window = obj as CreateTeacherNoAvaliable;
            window.Close();
        }

        /// <summary>
        /// 获取禁止时间
        /// </summary>
        /// <returns></returns>
        List<DayPeriodModel> getforbidTimes()
        {
            List<DayPeriodModel> forbids = new List<DayPeriodModel>();

            var periods = this.Periods.Where(p =>
            p.PositionType != XYKernel.OS.Common.Enums.Position.AB &&
            p.PositionType != XYKernel.OS.Common.Enums.Position.PB &&
            p.PositionType != XYKernel.OS.Common.Enums.Position.Noon);

            if (periods != null)
            {
                foreach (var p in periods)
                {
                    if (!p.Monday.IsChecked)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Monday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (!p.Tuesday.IsChecked)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Tuesday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (!p.Wednesday.IsChecked)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Wednesday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (!p.Thursday.IsChecked)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Thursday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (!p.Friday.IsChecked)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Friday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (!p.Saturday.IsChecked)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Saturday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (!p.Sunday.IsChecked)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Sunday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                }
            }

            return forbids;
        }

        void getBase(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule)
        {
            base.OpratorEnum = opratorEnum;
            this.TitleString = $"{ opratorEnum.GetLocalDescription()}-{timeRule.GetLocalDescription()}";
            this.RaisePropertyChanged(() => ShowRead);
            this.RaisePropertyChanged(() => ShowEdit);

            var cl = base.GetClCase(base.LocalID);

            this.Teachers = cl.Teachers.Select(t =>
            {
                return new UITeacher()
                {
                    ID = t.ID,
                    Name = t.Name
                };
            })?.ToList();

            this.SelectTeacher = this.Teachers.FirstOrDefault();

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

                    if (first.Position != XYKernel.OS.Common.Enums.Position.AB
                       && first.Position != XYKernel.OS.Common.Enums.Position.PB
                       && first.Position != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        g.ToList().ForEach(gg =>
                        {
                            if (gg.DayPeriod.Day == DayOfWeek.Monday)
                            {
                                week.Monday.IsChecked = gg.IsSelected;
                            }
                            else if (gg.DayPeriod.Day == DayOfWeek.Tuesday)
                            {
                                week.Tuesday.IsChecked = gg.IsSelected;
                            }
                            else if (gg.DayPeriod.Day == DayOfWeek.Wednesday)
                            {
                                week.Wednesday.IsChecked = gg.IsSelected;
                            }
                            else if (gg.DayPeriod.Day == DayOfWeek.Thursday)
                            {
                                week.Thursday.IsChecked = gg.IsSelected;
                            }
                            else if (gg.DayPeriod.Day == DayOfWeek.Friday)
                            {
                                week.Friday.IsChecked = gg.IsSelected;
                            }
                            else if (gg.DayPeriod.Day == DayOfWeek.Saturday)
                            {
                                week.Saturday.IsChecked = gg.IsSelected;
                            }
                            else if (gg.DayPeriod.Day == DayOfWeek.Sunday)
                            {
                                week.Sunday.IsChecked = gg.IsSelected;
                            }
                        });
                    }

                    results.Add(week);
                }
            }
            this.Periods = results;
        }

        void bind(UITeacherRule receive)
        {
            this.UID = receive.UID;
            this.Weight = receive.Weight;
            this.IsActive = receive.IsActive;
            this.SelectTeacher = this.Teachers.FirstOrDefault(t => t.ID.Equals(receive.TeacherID));

            receive.ForbidTimes.ForEach(t =>
            {
                var period = this.Periods.First(p => p.Period.Period == t.Period);
                if (period != null)
                {
                    if (t.Day == DayOfWeek.Monday)
                    {
                        period.Monday.IsChecked = false;
                    }
                    else if (t.Day == DayOfWeek.Tuesday)
                    {
                        period.Tuesday.IsChecked = false;
                    }
                    else if (t.Day == DayOfWeek.Wednesday)
                    {
                        period.Wednesday.IsChecked = false;
                    }
                    else if (t.Day == DayOfWeek.Thursday)
                    {
                        period.Thursday.IsChecked = false;
                    }
                    else if (t.Day == DayOfWeek.Friday)
                    {
                        period.Friday.IsChecked = false;
                    }
                    else if (t.Day == DayOfWeek.Saturday)
                    {
                        period.Saturday.IsChecked = false;
                    }
                    else if (t.Day == DayOfWeek.Sunday)
                    {
                        period.Sunday.IsChecked = false;
                    }
                }
            });
        }

        public void SetValue(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule)
        {
            this.getBase(opratorEnum, timeRule);
        }

        public void SetValue(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule, UITeacherRule rule)
        {
            this.getBase(opratorEnum, timeRule);

            this.bind(rule);
        }

        [InjectionMethod]
        public void Initilize()
        {

        }
    }
}
