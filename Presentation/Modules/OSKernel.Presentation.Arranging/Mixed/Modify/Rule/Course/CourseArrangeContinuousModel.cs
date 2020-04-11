using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Time;
using Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course.Model;
using OSKernel.Presentation.CustomControl;
using XYKernel.OS.Common.Models;
using OSKernel.Presentation.Models.Mixed;
using OSKernel.Presentation.Models.Enums;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course
{
    public class CourseArrangeContinuousModel : CommonViewModel, IInitilize
    {
        private List<UITwoStatusWeek> _periods;

        private bool _isContinous = false;

        private bool _unContinous = true;

        private bool _isShowInterval;

        private int _continousCount = 1;

        private bool _isIntervalDay;

        private bool _isNoCrossingBreak = true;

        private List<UICourseClass> _courseClasses;

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

        public ICommand ModifyCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(modifyCommand);
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
        /// 是否连排
        /// </summary>
        public bool IsContinous
        {
            get
            {
                return _isContinous;
            }

            set
            {
                _isContinous = value;
                RaisePropertyChanged(() => IsContinous);
            }
        }

        /// <summary>
        /// 不连排
        /// </summary>
        public bool UnContinous
        {
            get
            {
                return _unContinous;
            }

            set
            {
                _unContinous = value;
                RaisePropertyChanged(() => UnContinous);
            }
        }

        /// <summary>
        /// 连排数量
        /// </summary>
        public int ContinousCount
        {
            get
            {
                return _continousCount;
            }

            set
            {
                if (value < 0)
                    value = 1;
                else if (value > 5)
                    value = 5;

                _continousCount = value;
                RaisePropertyChanged(() => ContinousCount);

                if (ContinousCount > 0)
                {
                    this.IsContinous = true;

                    if (ContinousCount >= 2)
                    {
                        this.IsShowInterval = true;
                    }
                    else
                    {
                        this.IsShowInterval = false;
                    }
                }
            }
        }

        /// <summary>
        /// 隔天
        /// </summary>
        public bool IsIntervalDay
        {
            get
            {
                return _isIntervalDay;
            }

            set
            {
                _isIntervalDay = value;
                RaisePropertyChanged(() => IsIntervalDay);
            }
        }

        /// <summary>
        /// 不跨上下午大课间
        /// </summary>
        public bool IsNoCrossingBreak
        {
            get
            {
                return _isNoCrossingBreak;
            }

            set
            {
                _isNoCrossingBreak = value;
                RaisePropertyChanged(() => IsNoCrossingBreak);
            }
        }

        /// <summary>
        /// 课程班级列表
        /// </summary>
        public List<UICourseClass> CourseClasses
        {
            get
            {
                return _courseClasses;
            }

            set
            {
                _courseClasses = value;
                RaisePropertyChanged(() => CourseClasses);
            }
        }

        /// <summary>
        /// 是否显示隔天
        /// </summary>
        public bool IsShowInterval
        {
            get
            {
                return _isShowInterval;
            }

            set
            {
                _isShowInterval = value;
                RaisePropertyChanged(() => IsShowInterval);
            }
        }


        public CourseArrangeContinuousModel()
        {
            this.Periods = new List<UITwoStatusWeek>();
            this.CourseClasses = new List<UICourseClass>();

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

            this.Comments = CommonDataManager.GetMixedRuleComments(MixedRuleEnum.CourseArrangeContinuous);

            var cl = base.GetClCase(base.LocalID);
            var rule = base.GetClRule(base.LocalID);

            #region  绑定时间

            this.RefreshPosition();

            #endregion

            #region 获取所有科目

            // 1.绑定列表
            var courses = new List<UICourseClass>();
            cl.Courses.ForEach(c =>
            {
                UICourseClass ui = new UICourseClass()
                {
                    CourseID = c.ID,
                    Name = c.Name,
                    LevelName = c.Name
                };
                ui.Classes = cl.GetClasses(c.ID);

                //2.绑定状态
                ui.Classes.ForEach(cc =>
                {
                    var classModel = rule.ArrangeContinuous.FirstOrDefault(ca => ca.ClassID.Equals(cc.ID));
                    if (classModel != null)
                    {
                        cc.HasOperation = true;
                    }
                });
                courses.Add(ui);

                // 注册事件
                ui.PropertyChanged += Ui_PropertyChanged;
            });
            this.CourseClasses = courses;

            #endregion
        }

        private void Ui_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UICourseClass ui = sender as UICourseClass;
            if (e.PropertyName.Equals(nameof(ui.IsChecked)))
            {
                ui.Classes.ForEach(c =>
                {
                    c.IsChecked = ui.IsChecked;
                });
            }
        }

        void modifyCommand(object obj)
        {
            UIClass classModel = obj as UIClass;

            this.IsContinous = true;

            // 清除选中
            this.CourseClasses.ForEach(c =>
            {
                c.Classes.ForEach(cc =>
                {
                    cc.IsChecked = false;
                });
            });

            // 选中当前项
            classModel.IsChecked = true;

            // 清除选中状态
            this.Periods.ForEach(p => p.SetStatus(false));

            // 绑定状态
            var rule = base.GetClRule(base.LocalID);

            var result = rule.ArrangeContinuous.FirstOrDefault(a => a.ClassID.Equals(classModel.ID));
            result.Times.ForEach(t =>
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

            this.ContinousCount = result.Count;
            this.IsIntervalDay = result.IsIntervalDay;
            this.IsNoCrossingBreak = result.NoCrossingBreak;

            base.SelectWeight = (WeightTypeEnum)result.TimesWeight;
        }

        void userControlCommand(string parms)
        {
            if (parms.Equals("loaded"))
            {

            }
            else if (parms.Equals("unloaded"))
            {
                Messenger.Default.Unregister<HostView>(this, save);

                CourseClasses.ForEach(cc =>
                {
                    cc.PropertyChanged -= Ui_PropertyChanged;
                });
            }
        }

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

        void save(HostView host)
        {
            var rule = base.GetClRule(base.LocalID);
            var cl = base.GetClCase(base.LocalID);

            var selectClasses = (from c in this.CourseClasses
                                 from cc in c.Classes
                                 where cc.IsChecked
                                 select new
                                 {
                                     CourseID = c.CourseID,
                                     ClassID = cc.ID,
                                     c = cc,
                                 })?.ToList();

            if (selectClasses?.Count == 0)
            {
                this.ShowDialog("提示信息", "请选择班级", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            if (this.UnContinous)
            {
                // 不连排
                selectClasses.ForEach(sc =>
                {
                    rule.ArrangeContinuous.RemoveAll(r => r.ClassID.Equals(sc.ClassID));
                    sc.c.HasOperation = false;
                });

                rule.Serialize(base.LocalID);
                this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
                return;
            }

            // 获取时间
            var times = this.getTimes();

            selectClasses.ForEach(sc =>
            {
                var courseArrange = rule.ArrangeContinuous.FirstOrDefault(ca => ca.ClassID.Equals(sc.ClassID));
                if (courseArrange != null)
                {
                    courseArrange.Count = this.ContinousCount;
                    courseArrange.IntervalDayWeight = (int)base.SelectWeight;
                    courseArrange.IsIntervalDay = this.IsIntervalDay;
                    courseArrange.NoCrossingBreak = this.IsNoCrossingBreak;
                    courseArrange.NoCrossingBreakWeight = (int)base.SelectWeight;
                    courseArrange.Times = times;
                    courseArrange.TimesWeight = (int)base.SelectWeight;
                }
                else
                {
                    var courseContinous = new XYKernel.OS.Common.Models.Mixed.Rule.ArrangeContinousRule()
                    {
                        ClassID = sc.ClassID,
                        Count = this.ContinousCount,
                        IntervalDayWeight = (int)base.SelectWeight,
                        IsIntervalDay = this.IsIntervalDay,
                        NoCrossingBreak = this.IsNoCrossingBreak,
                        NoCrossingBreakWeight = (int)base.SelectWeight,
                        Times = times,
                        TimesWeight = (int)base.SelectWeight,
                    };
                    rule.ArrangeContinuous.Add(courseContinous);
                }
                sc.c.HasOperation = true;
            });

            base.SerializePatternRule(rule, base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);

            // 清除显示状态
            this.CourseClasses.ForEach(cc =>
            {
                cc.IsChecked = false;
            });

            // 清除课位
            this.RefreshPosition();
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
