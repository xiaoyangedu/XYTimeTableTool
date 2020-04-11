using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Models.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Mixed.Rule;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher
{
    public class TeacherTimeViewModel : CommonViewModel, IInitilize
    {
        private bool _isAllChecked;

        private List<UITeacher> _teachers;

        private List<UITwoStatusWeek> _periods;

        private List<XYKernel.OS.Common.Models.Mixed.Rule.TeacherTimeRule> _teacherTimes;

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public ICommand ModifyCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(modifyCommand);
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

        public bool IsAllChecked
        {
            get
            {
                return _isAllChecked;
            }

            set
            {
                _isAllChecked = value;
                RaisePropertyChanged(() => IsAllChecked);

                this.Teachers.ForEach(t =>
                {
                    t.IsChecked = _isAllChecked;
                });
            }
        }

        public TeacherTimeViewModel()
        {
            this.Teachers = new List<UITeacher>();
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
            Messenger.Default.Register<HostView>(this, save);

            this.Comments = CommonDataManager.GetMixedRuleComments(MixedRuleEnum.TeacherTime);

            var cl = base.GetClCase(base.LocalID);
            var rule = base.GetClRule(base.LocalID);

            this._teacherTimes = rule.TeacherTimes;

            #region 绑定教师

            this.Teachers = cl.Teachers.Select(t =>
              {
                  return new UITeacher()
                  {
                      ID = t.ID,
                      Name = t.Name,
                  };
              })?.ToList();

            this.Teachers.ForEach(t =>
            {
                var result = rule.TeacherTimes.FirstOrDefault(tt => tt.TeacherID.Equals(t.ID));
                if (result != null)
                {
                    t.HasOperation = true;
                    t.Weight = (WeightTypeEnum)result.Weight;
                }
            });

            #endregion

            #region 绑定课位

            this.RefreshPosition();

            #endregion
        }

        void userControlCommand(string parms)
        {
            if (parms.Equals("loaded"))
            {

            }
            else if (parms.Equals("unloaded"))
            {
                Messenger.Default.Unregister<HostView>(this, save);
            }
        }

        void modifyCommand(object obj)
        {
            UITeacher teacher = obj as UITeacher;
            this.bindData(teacher);
        }

        void save(HostView host)
        {
            var rule = base.GetClRule(base.LocalID);

            var has = this.Teachers.Any(t => t.IsChecked);
            if (!has)
            {
                this.ShowDialog("提示信息", "没有选择教师", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            List<DayPeriodModel> allows = new List<DayPeriodModel>();
            List<DayPeriodModel> forbids = new List<DayPeriodModel>();

            var periods = this.Periods.Where(p =>
              p.PositionType != XYKernel.OS.Common.Enums.Position.AB &&
              p.PositionType != XYKernel.OS.Common.Enums.Position.PB &&
              p.PositionType != XYKernel.OS.Common.Enums.Position.Noon);

            if (periods != null)
            {
                foreach (var p in periods)
                {
                    if (p.Monday.IsChecked && p.Monday.IsMouseLeft)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Monday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                    else if (p.Monday.IsChecked && !p.Monday.IsMouseLeft)
                    {
                        allows.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Monday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Tuesday.IsChecked && p.Tuesday.IsMouseLeft)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Tuesday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                    else if (p.Tuesday.IsChecked && !p.Tuesday.IsMouseLeft)
                    {
                        allows.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Tuesday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                    if (p.Wednesday.IsChecked && p.Wednesday.IsMouseLeft)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Wednesday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                    else if (p.Wednesday.IsChecked && !p.Wednesday.IsMouseLeft)
                    {
                        allows.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Wednesday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Thursday.IsChecked && p.Thursday.IsMouseLeft)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Thursday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                    else if (p.Thursday.IsChecked && !p.Thursday.IsMouseLeft)
                    {
                        allows.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Thursday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Friday.IsChecked && p.Friday.IsMouseLeft)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Friday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                    else if (p.Friday.IsChecked && !p.Friday.IsMouseLeft)
                    {
                        allows.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Friday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Saturday.IsChecked && p.Saturday.IsMouseLeft)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Saturday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                    else if (p.Saturday.IsChecked && !p.Saturday.IsMouseLeft)
                    {
                        allows.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Saturday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }

                    if (p.Sunday.IsChecked && p.Sunday.IsMouseLeft)
                    {
                        forbids.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Sunday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                    else if (p.Sunday.IsChecked && !p.Sunday.IsMouseLeft)
                    {
                        allows.Add(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Sunday,
                            Period = p.Period.Period,
                            PeriodName = p.Period.PeriodName
                        });
                    }
                }

                var teachers = this.Teachers.Where(t => t.IsChecked);

                foreach (var teacher in teachers)
                {
                    var teacherRule = rule.TeacherTimes.FirstOrDefault(t => t.TeacherID.Equals(teacher.ID));
                    if (teacherRule == null)
                    {
                        teacherRule = new TeacherTimeRule();
                        rule.TeacherTimes.Add(teacherRule);
                    }
                    teacherRule.ForbidTimes = forbids;
                    teacherRule.MustTimes = allows;
                    teacherRule.TeacherID = teacher.ID;
                    teacherRule.Weight = (int)teacher.Weight;

                    // 更新状态
                    if (forbids.Count > 0 || allows.Count > 0)
                        teacher.HasOperation = true;
                    else
                    {
                        teacher.HasOperation = false;
                        rule.TeacherTimes.Remove(teacherRule);
                    }
                }

                base.SerializePatternRule(rule, base.LocalID);
                this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
            }


            // 刷新状态
            this.Teachers.ForEach(t =>
            {
                t.IsChecked = false;
            });

            this.RefreshPosition();
        }

        void bindData(UITeacher teacher)
        {
            this.Teachers.ForEach(t =>
            {
                t.IsChecked = false;
            });
            teacher.IsChecked = true;

            this.Periods.ForEach(p =>
            {
                p.ClearStatus();
            });

            var rule = base.GetClRule(base.LocalID);
            if (rule != null)
            {
                var teacherTime = rule.TeacherTimes.FirstOrDefault(t => t.TeacherID.Equals(teacher.ID));
                if (teacherTime != null)
                {
                    teacherTime.ForbidTimes.ForEach(t =>
                    {
                        var period = this.Periods.First(p => p.Period.Period == t.Period);
                        if (period != null)
                        {
                            if (t.Day == DayOfWeek.Monday)
                            {
                                period.Monday.IsChecked = true;
                                period.Monday.IsMouseLeft = true;
                            }
                            else if (t.Day == DayOfWeek.Tuesday)
                            {
                                period.Tuesday.IsChecked = true;
                                period.Tuesday.IsMouseLeft = true;
                            }
                            else if (t.Day == DayOfWeek.Wednesday)
                            {
                                period.Wednesday.IsChecked = true;
                                period.Wednesday.IsMouseLeft = true;
                            }
                            else if (t.Day == DayOfWeek.Thursday)
                            {
                                period.Thursday.IsChecked = true;
                                period.Thursday.IsMouseLeft = true;
                            }
                            else if (t.Day == DayOfWeek.Friday)
                            {
                                period.Friday.IsChecked = true;
                                period.Friday.IsMouseLeft = true;
                            }
                            else if (t.Day == DayOfWeek.Saturday)
                            {
                                period.Saturday.IsChecked = true;
                                period.Saturday.IsMouseLeft = true;
                            }
                            else if (t.Day == DayOfWeek.Sunday)
                            {
                                period.Sunday.IsChecked = true;
                                period.Sunday.IsMouseLeft = true;
                            }
                        }
                    });

                    teacherTime.MustTimes.ForEach(t =>
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
            }
        }

        public override void BatchSetWeight(WeightTypeEnum weightEnum)
        {
            this.Teachers.ForEach(r =>
            {
                r.Weight = weightEnum;
            });
        }

        void RefreshPosition()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

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
                        PositionType = first.Position
                    };
                    results.Add(week);
                }
            }

            this.Periods = results;
        }
    }
}
