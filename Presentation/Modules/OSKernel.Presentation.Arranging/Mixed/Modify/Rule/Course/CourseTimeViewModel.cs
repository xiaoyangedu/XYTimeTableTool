using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Models.Mixed;
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

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course
{
    public class CourseTimeViewModel : CommonViewModel, IInitilize
    {
        private List<UITwoStatusWeek> _periods;

        private List<UICourseClass> _courseClasses;

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
        public CourseTimeViewModel()
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

            this.Comments = CommonDataManager.GetMixedRuleComments(MixedRuleEnum.CourseTime);

            var cl = base.GetClCase(base.LocalID);
            var rule = base.GetClRule(base.LocalID);

            this.RefreshPosition();

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
                    var classModel = rule.CourseTimes.FirstOrDefault(ca => ca.ClassID.Equals(cc.ID));
                    if (classModel != null)
                    {
                        cc.HasOperation = true;
                    }
                });
                courses.Add(ui);
            });
            this.CourseClasses = courses;

            #endregion

            // 课程班级列表
            this.CourseClasses.ForEach(cc =>
            {
                cc.PropertyChanged += cc_PropertyChanged;
            });
        }

        private void cc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UICourseClass uicc = sender as UICourseClass;
            if (e.PropertyName.Equals(nameof(uicc.IsChecked)))
            {
                uicc.Classes.ForEach(c =>
                {
                    c.IsChecked = uicc.IsChecked;
                });
            }
        }

        void modifyCommand(object obj)
        {
            UIClass classModel = obj as UIClass;

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

            var result = rule.CourseTimes.FirstOrDefault(a => a.ClassID.Equals(classModel.ID));

            if (result != null)
            {
                result.ForbidTimes.ForEach(t =>
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

                result.MustTimes.ForEach(t =>
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

                base.SelectWeight = (WeightTypeEnum)result.Weight;
            }
        }

        void userControlCommand(string parms)
        {
            if (parms.Equals("loaded"))
            {

            }
            else if (parms.Equals("unloaded"))
            {
                Messenger.Default.Unregister<HostView>(this, save);

                this.CourseClasses.ForEach(c =>
                {
                    c.PropertyChanged -= cc_PropertyChanged;
                });
            }
        }

        void save(HostView host)
        {
            var rule = base.GetClRule(base.LocalID);

            List<DayPeriodModel> allows = new List<DayPeriodModel>();
            List<DayPeriodModel> forbids = new List<DayPeriodModel>();

            var periods = this.Periods.Where(p =>
              p.PositionType != XYKernel.OS.Common.Enums.Position.AB &&
              p.PositionType != XYKernel.OS.Common.Enums.Position.PB &&
              p.PositionType != XYKernel.OS.Common.Enums.Position.Noon);

            var selectClasses = (from c in this.CourseClasses
                                 from cc in c.Classes
                                 where cc.IsChecked
                                 select new
                                 {
                                     CourseID = c.CourseID,
                                     ClassID = cc.ID,
                                     LevelID = cc.LevelID,
                                     cc

                                 })?.ToList();

            if (selectClasses?.Count == 0)
            {
                this.ShowDialog("提示信息", "请选择课程", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

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

                selectClasses.ForEach(sc =>
                {
                    var first = rule.CourseTimes.FirstOrDefault(ct => ct.ClassID.Equals(sc.ClassID));
                    if (first != null)
                    {
                        if (forbids.Count == 0 && allows.Count == 0)
                        {
                            rule.CourseTimes.Remove(first);
                            sc.cc.HasOperation = false;
                        }
                        else
                        {
                            first.ForbidTimes = forbids;
                            first.MustTimes = allows;
                            first.Weight = (int)base.SelectWeight;
                        }
                    }
                    else
                    {
                        var courseTime = new CourseTimeRule()
                        {
                            ClassID = sc.ClassID,
                            ForbidTimes = forbids,
                            MustTimes = allows,
                            Weight = (int)base.SelectWeight
                        };
                        rule.CourseTimes.Add(courseTime);
                        sc.cc.HasOperation = true;
                    }

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
