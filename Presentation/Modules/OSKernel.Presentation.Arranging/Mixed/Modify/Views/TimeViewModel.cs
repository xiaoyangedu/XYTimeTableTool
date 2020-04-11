using System;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Base;
using System.Collections.ObjectModel;
using OSKernel.Presentation.Core;
using Unity;
using OSKernel.Presentation.CustomControl;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views
{
    /// <summary>
    /// 课位ViewModel
    /// </summary>
    public class TimeViewModel : CommonViewModel, IInitilize, IRefresh
    {
        private bool _morningStudyChecked;

        private bool _amChecked;

        private bool _amBreakChecked;

        private bool _afternoonChecked;

        private bool _pmChecked;

        private bool _pmBreakChecked;

        private bool _eveningStudyChecked;

        private bool _isLoaded;

        private ObservableCollection<UIPosition> _positions;

        private Dictionary<int, string> _periods;

        /// <summary>
        /// 上午大课间可用
        /// </summary>
        public bool AmBreakEnable
        {
            get
            {
                if (_amChecked == true)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 下午大课间可用
        /// </summary>
        public bool PmBreakEnable
        {
            get
            {
                if (_pmChecked == true)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 午休按钮可用
        /// </summary>
        public bool NoonEnable
        {
            get
            {
                if (_amChecked && _pmChecked)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(saveCommand);
            }
        }

        public ICommand InsertTimeCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(insertTime);
            }
        }

        public ICommand DeleteTimeCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(deleteTime);
            }
        }

        public ICommand PreviousCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(previous);
            }
        }

        public ICommand DownCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(downTime);
            }
        }

        public bool MorningStudyChecked
        {
            get
            {
                return _morningStudyChecked;
            }

            set
            {
                _morningStudyChecked = value;
                RaisePropertyChanged(() => MorningStudyChecked);

                // 不为加载的时候触发
                if (!_isLoaded)
                    ms(value);
            }
        }

        public bool AmChecked
        {
            get
            {
                return _amChecked;
            }

            set
            {
                _amChecked = value;
                RaisePropertyChanged(() => AmChecked);
                RaisePropertyChanged(() => AmBreakEnable);
                RaisePropertyChanged(() => NoonEnable);

                // 不为加载的时候触发
                if (!_isLoaded)
                {
                    am(value);

                    if (!value)
                    {
                        AmBreakChecked = false;
                    }

                    if (!PmChecked || !AmChecked)
                    {
                        AfternoonChecked = false;
                    }
                }
            }
        }

        public bool AmBreakChecked
        {
            get
            {
                return _amBreakChecked;
            }

            set
            {
                _amBreakChecked = value;
                RaisePropertyChanged(() => AmBreakChecked);

                // 不为加载的时候触发
                if (!_isLoaded)
                    amBreak(value);
            }
        }

        public bool AfternoonChecked
        {
            get
            {
                return _afternoonChecked;
            }

            set
            {
                _afternoonChecked = value;
                RaisePropertyChanged(() => AfternoonChecked);

                // 不为加载的时候触发
                if (!_isLoaded)
                    noon(value);
            }
        }

        public bool PmChecked
        {
            get
            {
                return _pmChecked;
            }

            set
            {
                _pmChecked = value;
                RaisePropertyChanged(() => PmChecked);
                RaisePropertyChanged(() => PmBreakEnable);
                RaisePropertyChanged(() => NoonEnable);

                // 不为加载的时候触发
                if (!_isLoaded)
                {
                    pm(value);

                    if (!value)
                    {
                        PmBreakChecked = false;
                    }

                    if (!PmChecked || !AmChecked)
                    {
                        AfternoonChecked = false;
                    }
                }
            }
        }

        public bool PmBreakChecked
        {
            get
            {
                return _pmBreakChecked;
            }

            set
            {
                _pmBreakChecked = value;
                RaisePropertyChanged(() => PmBreakChecked);

                // 不加载数据的时候
                if (!_isLoaded)
                    pmBreak(value);
            }
        }

        public bool EveningStudyChecked
        {
            get
            {
                return _eveningStudyChecked;
            }

            set
            {
                _eveningStudyChecked = value;
                RaisePropertyChanged(() => EveningStudyChecked);

                if (!_isLoaded)
                    es(value);
            }
        }

        /// <summary>
        /// 课位信息
        /// </summary>
        public ObservableCollection<UIPosition> Positions
        {
            get
            {
                return _positions;
            }

            set
            {
                _positions = value;
                RaisePropertyChanged(() => Positions);
            }
        }

        public TimeViewModel()
        {
            Positions = new ObservableCollection<UIPosition>();
            _periods = new Dictionary<int, string>
            {
                { 1,"第一节"},
                { 2,"第二节"},
                { 3,"第三节"},
                { 4,"第四节"},
                { 5,"第五节"},
                { 6,"第六节"},
                { 7,"第七节"},
                { 8,"第八节"},
                { 9,"第九节"},
                { 10,"第十节"},
                { 11,"第十一节"},
                { 12,"第十二节"},
                { 13,"第十三节"},
                { 14,"第十四节"},
                { 15,"第十五节"},
            };
        }

        [InjectionMethod]
        public void Initilize()
        {
            this.Positions.Clear();

            var cl = base.GetClCase(base.LocalID);

            this._isLoaded = true;
            var positions = cl.Positions;

            #region CheckBox 选中状态

            // 早自习
            var ms = positions.Any(p => p.Position == XYKernel.OS.Common.Enums.Position.MS);
            this.MorningStudyChecked = ms;

            // 上午   
            var am = positions.Any(p => p.Position == XYKernel.OS.Common.Enums.Position.AM);
            this.AmChecked = am;

            // 上午大课间
            var ab = positions.Any(p => p.Position == XYKernel.OS.Common.Enums.Position.AB);
            this.AmBreakChecked = ab;

            // 中午
            var noon = positions.Any(p => p.Position == XYKernel.OS.Common.Enums.Position.Noon);
            this.AfternoonChecked = noon;

            // 下午
            var pm = positions.Any(p => p.Position == XYKernel.OS.Common.Enums.Position.PM);
            this.PmChecked = pm;

            // 下午大课间
            var pb = positions.Any(p => p.Position == XYKernel.OS.Common.Enums.Position.PB);
            this.PmBreakChecked = pb;

            // 晚自习
            var es = positions.Any(p => p.Position == XYKernel.OS.Common.Enums.Position.ES);
            this.EveningStudyChecked = es;

            this._isLoaded = false;

            #endregion

            #region 绑定课位

            var groups = positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);
            foreach (var g in groups)
            {
                Dictionary<DayOfWeek, bool> weekSelected = new Dictionary<DayOfWeek, bool>();
                g.ToList().ForEach(gg =>
                {
                    weekSelected.Add(gg.DayPeriod.Day, gg.IsSelected);
                });

                var first = g.First();
                var periodString = first.DayPeriod.PeriodName;

                var position = new UIPosition()
                {
                    PositionType = first.Position,
                    IsMondayChecked = weekSelected[DayOfWeek.Monday],
                    IsTuesdayChecked = weekSelected[DayOfWeek.Tuesday],
                    IsWednesdayChecked = weekSelected[DayOfWeek.Wednesday],
                    IsThursdayChecked = weekSelected[DayOfWeek.Thursday],
                    IsFridayChecked = weekSelected[DayOfWeek.Friday],
                    IsSaturdayChecked = weekSelected[DayOfWeek.Saturday],
                    IsSundayChecked = weekSelected[DayOfWeek.Sunday],
                    PeriodString = periodString
                };
                this.Positions.Add(position);
            }

            // 让最后一个选项可用
            var msLast = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.MS);
            if (msLast != null)
                msLast.CanOperation = true;

            var abLast = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AB);
            if (abLast != null)
                abLast.CanOperation = true;

            var amLast = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
            if (amLast != null)
                amLast.CanOperation = true;

            var pbLast = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PB);
            if (pbLast != null)
                pbLast.CanOperation = true;

            var pmLast = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);
            if (pmLast != null)
                pmLast.CanOperation = true;

            var esLast = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.ES);
            if (esLast != null)
                esLast.CanOperation = true;

            #endregion

            //this.MorningStudyChecked = true;
            //this.AmChecked = true;
            //this.AmBreakChecked = true;
            //this.PmChecked = true;
            //this.AfternoonChecked = true;
            //this.PmBreakChecked = true;
            //this.EveningStudyChecked = true;
        }

        void saveCommand()
        {
            StringBuilder information = new StringBuilder();
            information.AppendLine("调整课位会清空以下规则,是否确认修改?");
            information.AppendLine();
            information.AppendLine("规则-课程连排");
            information.AppendLine("规则-教师排课时间");
            information.AppendLine("规则-同时开课限制");
            information.AppendLine("规则-课程排课时间");
            information.AppendLine();
            information.AppendLine("高级-教师的不可用时间");
            information.AppendLine("高级-单个课时有多个优先课位");
            information.AppendLine("高级-单个课时有多个优先开始时间");
            information.AppendLine("高级-单个课时有一个优先开始时间");
            information.AppendLine("高级-多个课时有多个优先课位");
            information.AppendLine("高级-多个课时有多个优先开始时间");
            information.AppendLine("高级-多个课时在选定课位中占用的最大数量");
            information.AppendLine("高级-在选定课位中设置多个课时的最大同时开课数量");

            var confirm = this.ShowDialog("提示信息", information.ToString(), CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (confirm != CustomControl.Enums.DialogResultType.OK)
            {
                return;
            }
            MixedDataHelper.TimeChanged(base.LocalID, CommonDataManager);

            var cl = base.GetClCase(base.LocalID);

            //清除课位
            cl.Positions.Clear();

            // 遍历课位集合
            for (int i = 0; i < Positions.Count; i++)
            {
                var p = Positions[i];
                #region 周一至周日

                var monday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Monday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsMondayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(monday);

                var tuesday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Tuesday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsTuesdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(tuesday);

                var wednesday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Wednesday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsWednesdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(wednesday);

                var thursday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Thursday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsThursdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(thursday);

                var Friday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Friday,
                        Period = i,
                        PeriodName = p.PeriodString,
                    },
                    IsSelected = p.IsFridayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(Friday);

                var saturday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Saturday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsSaturdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(saturday);

                var sunday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Sunday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsSundayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(sunday);

                #endregion
            }

            base.Serialize(cl, LocalID);

            // TODO 保存提示
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        /// <summary>
        /// 早自习
        /// </summary>
        void ms(bool isChecked)
        {
            if (isChecked)
            {
                UIPosition position1 = new UIPosition()
                {
                    PeriodString = "早自习一",
                    PositionType = XYKernel.OS.Common.Enums.Position.MS,
                    CanOperation = true,
                };
                this.Positions.Insert(0, position1);
            }
            else
            {
                var positions = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.MS)?.ToList();
                foreach (var p in positions)
                {
                    this.Positions.Remove(p);
                }
            }
        }

        /// <summary>
        /// 晚自习
        /// </summary>
        void es(bool isChecked)
        {
            if (isChecked)
            {
                int index = 0;
                var lastPM = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);

                if (lastPM != null)
                {
                    // 下午
                    index = this.Positions.IndexOf(lastPM);
                }
                else
                {
                    var noon = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.Noon);
                    if (noon != null)
                    {
                        // 中午
                        index = this.Positions.IndexOf(noon);
                    }
                    else
                    {
                        var lastMS = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.MS);
                        if (lastMS != null)
                        {
                            // 早自习
                            index = this.Positions.IndexOf(lastMS);
                        }
                    }
                }

                UIPosition esPosition1 = new UIPosition()
                {
                    PeriodString = "晚自习一",
                    PositionType = XYKernel.OS.Common.Enums.Position.ES,
                };

                UIPosition esPosition2 = new UIPosition()
                {
                    PeriodString = "晚自习二",
                    PositionType = XYKernel.OS.Common.Enums.Position.ES,
                    CanOperation = true,
                };

                if (index == 0)
                {
                    this.Positions.Insert(0, esPosition1);
                    this.Positions.Insert(1, esPosition2);
                }
                else
                {
                    this.Positions.Insert(index + 1, esPosition1);
                    this.Positions.Insert(index + 2, esPosition2);
                }
            }
            else
            {
                var positions = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.ES)?.ToList();

                foreach (var p in positions)
                {
                    this.Positions.Remove(p);
                }
            }
        }

        /// <summary>
        /// 上午
        /// </summary>
        void am(bool isChecked)
        {
            if (isChecked)
            {
                // 找早自习
                var msPositions = Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.MS);
                var msIndex = msPositions == null ? 0 : msPositions.Count();

                for (int i = 0; i < 4; i++)
                {
                    UIPosition position = new UIPosition()
                    {
                        PeriodString = _periods[i + 1],
                        PositionType = XYKernel.OS.Common.Enums.Position.AM
                    };
                    this.Positions.Insert(msIndex + i, position);
                }

                var last = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                last.CanOperation = true;

                // 2.如果存在下午将下午更改显示名称
                var pms = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM)?.ToList();
                var pmsCount = pms.Count();
                if (pmsCount > 0)
                {
                    // 上午节次默认4节
                    for (int i = 1; i <= pmsCount; i++)
                    {
                        pms[i - 1].PeriodString = _periods[4 + i];
                    }
                }
            }
            else
            {
                // 移除上午课时
                var amPositoins = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM)?.ToList();
                foreach (var position in amPositoins)
                {
                    this.Positions.Remove(position);
                }

                // 修改下午节次名称
                var amCount = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                var pms = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM)?.ToList();
                var pmsCount = pms.Count();
                if (pmsCount > 0)
                {
                    // 上午节次默认4节
                    for (int i = 1; i <= pmsCount; i++)
                    {
                        pms[i - 1].PeriodString = _periods[amCount + i];
                    }
                }
            }
        }

        /// <summary>
        /// 下午
        /// </summary>
        void pm(bool isChecked)
        {
            if (isChecked)
            {
                int index = 0;

                // 找午休
                var noon = Positions.FirstOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.Noon);
                if (noon == null)
                {
                    var lastAM = Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                    if (lastAM != null)
                    {
                        index = Positions.IndexOf(lastAM) + 1;
                    }
                    else
                    {
                        var lastMs = Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.MS);
                        if (lastMs != null)
                        {
                            index = Positions.IndexOf(lastMs) + 1;
                        }
                    }
                }
                else
                {
                    index = Positions.IndexOf(noon);
                }

                // 查找上午多少节
                var number = Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                for (int i = number; i < number + 4; i++)
                {
                    UIPosition position = new UIPosition()
                    {
                        PeriodString = _periods[i + 1],
                        PositionType = XYKernel.OS.Common.Enums.Position.PM
                    };
                    this.Positions.Insert(index++, position);
                }

                var last = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);
                last.CanOperation = true;
            }
            else
            {
                var amPositoins = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM)?.ToList();
                foreach (var position in amPositoins)
                {
                    this.Positions.Remove(position);
                }
            }
        }

        void amBreak(bool isChecked)
        {
            if (isChecked)
            {
                var amCount = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);

                if (amCount <= 1)
                {
                    this.AmBreakChecked = false;
                    return;
                }

                UIPosition breakPosition = new UIPosition()
                {
                    PeriodString = "上午大课间",
                    CanOperation = true,
                    PositionType = XYKernel.OS.Common.Enums.Position.AB,

                    IsFridayChecked = false,
                    IsMondayChecked = false,
                    IsThursdayChecked = false,
                    IsTuesdayChecked = false,
                    IsWednesdayChecked = false,
                    IsSaturdayChecked = false,
                    IsSundayChecked = false
                };

                var first = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM).FirstOrDefault();
                var index = this.Positions.IndexOf(first);

                if (amCount == 2)
                {
                    var lastPosition = this.Positions.LastOrDefault(l => l.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                    this.Positions.Remove(lastPosition);

                    this.Positions.Insert(index + 1, breakPosition);
                    this.Positions.Insert(index + 2, lastPosition);

                    return;
                }

                this.Positions.Insert(index + 2, breakPosition);
            }
            else
            {
                var amPositoins = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AB)?.ToList();
                foreach (var position in amPositoins)
                {
                    this.Positions.Remove(position);
                }
            }
        }

        void pmBreak(bool isChecked)
        {
            if (isChecked)
            {
                var pmCount = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);
                if (pmCount <= 1)
                {
                    this.PmBreakChecked = false;
                    return;
                }

                UIPosition breakPosition = new UIPosition()
                {
                    PeriodString = "下午大课间",
                    CanOperation = true,
                    PositionType = XYKernel.OS.Common.Enums.Position.PB,

                    IsFridayChecked = false,
                    IsMondayChecked = false,
                    IsThursdayChecked = false,
                    IsTuesdayChecked = false,
                    IsWednesdayChecked = false,
                    IsSaturdayChecked = false,
                    IsSundayChecked = false
                };

                var first = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM).FirstOrDefault();
                var index = this.Positions.IndexOf(first);

                if (pmCount == 2)
                {
                    var lastPosition = this.Positions.LastOrDefault(l => l.PositionType == XYKernel.OS.Common.Enums.Position.PM);
                    this.Positions.Remove(lastPosition);

                    this.Positions.Insert(index + 1, breakPosition);
                    this.Positions.Insert(index + 2, lastPosition);

                    return;
                }

                this.Positions.Insert(index + 2, breakPosition);
            }
            else
            {
                var amPositoins = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PB)?.ToList();
                foreach (var position in amPositoins)
                {
                    this.Positions.Remove(position);
                }
            }
        }

        /// <summary>
        /// 中午
        /// </summary>
        void noon(bool isChecked)
        {
            if (isChecked)
            {
                var last = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM).LastOrDefault();
                var index = this.Positions.IndexOf(last);

                UIPosition noon = new UIPosition()
                {
                    PeriodString = "午休",
                    CanOperation = true,
                    PositionType = XYKernel.OS.Common.Enums.Position.Noon,

                    IsFridayChecked = false,
                    IsMondayChecked = false,
                    IsThursdayChecked = false,
                    IsTuesdayChecked = false,
                    IsWednesdayChecked = false,
                    IsSaturdayChecked = false,
                    IsSundayChecked = false
                };
                this.Positions.Insert(index + 1, noon);
            }
            else
            {
                var amPositoins = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.Noon)?.ToList();
                foreach (var position in amPositoins)
                {
                    this.Positions.Remove(position);
                }
            }
        }

        void insertTime(object obj)
        {
            UIPosition position = obj as UIPosition;

            if (position.PositionType == XYKernel.OS.Common.Enums.Position.MS)
            {
                var count = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.MS);
                if (count == 2)
                {
                    this.ShowDialog("提示信息", "最多两节早自习!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    return;
                }

                var newPosition = position.Copy();
                newPosition.PeriodString = "早自习二";
                this.Positions.Insert(1, newPosition);

                position.CanOperation = false;

            }
            else if (position.PositionType == XYKernel.OS.Common.Enums.Position.AM)
            {
                var count = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                if (count == 6)
                {
                    this.ShowDialog("提示信息", "已达上限", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    return;
                }

                string periodString = "";
                if (count == 1)
                {
                    periodString = "第二节";
                }
                else if (count == 2)
                {
                    periodString = "第三节";
                }
                else if (count == 3)
                {
                    periodString = "第四节";
                }
                else if (count == 4)
                {
                    periodString = "第五节";
                }
                else if (count == 5)
                {
                    periodString = "第六节";
                }

                var last = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                position.CanOperation = false;
                int index = this.Positions.IndexOf(last);

                var newPosition = position.Copy();
                newPosition.PeriodString = periodString;
                newPosition.CanOperation = true;

                this.Positions.Insert(index + 1, newPosition);


                // 2.如果存在下午将下午更改显示名称
                var amCount = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                var pms = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM)?.ToList();
                var pmsCount = pms.Count();
                if (pmsCount > 0)
                {
                    // 上午节次默认4节
                    for (int i = 1; i <= pmsCount; i++)
                    {
                        pms[i - 1].PeriodString = _periods[amCount + i];
                    }
                }
            }
            else if (position.PositionType == XYKernel.OS.Common.Enums.Position.PM)
            {
                var count = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);
                if (count == 6)
                {
                    this.ShowDialog("提示信息", "已达上限", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    return;
                }

                string periodString = string.Empty;
                int lastIndex = 0;
                position.CanOperation = false;

                var lastPB = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);

                var filters = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM
                  || p.PositionType == XYKernel.OS.Common.Enums.Position.PM)?.ToList();

                lastIndex = filters.IndexOf(lastPB);
                if (lastIndex == -1)
                {
                    periodString = this._periods[0];
                }
                else
                {
                    periodString = this._periods[lastIndex + 2];
                }

                // 插入索引
                var insertIndex = this.Positions.IndexOf(position);

                // 创建新的对象
                var newPosition = position.Copy();
                newPosition.PeriodString = periodString;
                newPosition.CanOperation = true;

                // 插入
                this.Positions.Insert(insertIndex + 1, newPosition);
            }
            else if (position.PositionType == XYKernel.OS.Common.Enums.Position.ES)
            {
                var count = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.ES);
                if (count == 4)
                {
                    this.ShowDialog("提示信息", "晚自习最多四节!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    return;
                }

                string periodString = string.Empty;
                if (count == 1)
                {
                    periodString = "晚自习二";
                }
                else if (count == 2)
                {
                    periodString = "晚自习三";
                }
                else if (count == 3)
                {
                    periodString = "晚自习四";
                }

                position.CanOperation = false;

                var newPosition = position.Copy();
                newPosition.PeriodString = periodString;
                newPosition.CanOperation = true;
                this.Positions.Add(newPosition);
            }
        }

        void deleteTime(object obj)
        {
            UIPosition position = obj as UIPosition;

            if (position.PositionType == XYKernel.OS.Common.Enums.Position.MS)
            {
                this.Positions.Remove(position);

                var count = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.MS);
                if (count == 0)
                {
                    this.MorningStudyChecked = false;
                }
                else
                {
                    var last = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.MS);
                    last.PeriodString = "早自习一";
                    last.CanOperation = true;
                }
            }
            else if (position.PositionType == XYKernel.OS.Common.Enums.Position.AM)
            {
                var amBreak = this.Positions.FirstOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AB);
                // 没有大课间
                if (amBreak == null)
                {
                    this.Positions.Remove(position);

                    // 验证是否存在下午
                    var amCount = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                    var pms = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM)?.ToList();
                    var pmsCount = pms.Count();
                    if (pmsCount > 0)
                    {
                        for (int i = 1; i <= pmsCount; i++)
                        {
                            pms[i - 1].PeriodString = _periods[amCount + i];
                        }
                    }

                    var count = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                    if (count == 0)
                    {
                        this.AmChecked = false;
                        this.AfternoonChecked = false;
                    }
                    else
                    {
                        var last = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                        last.CanOperation = true;
                    }
                }
                else
                {
                    // 常规删除
                    this.Positions.Remove(position);

                    var last = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                    last.CanOperation = true;

                    // 删除上午大课间
                    var count = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                    if (count == 1)
                    {
                        this.AmBreakChecked = false;
                        this.Positions.Remove(amBreak);
                    }
                    else
                    {
                        // 验证大课间是否挨着节次
                        var currentIndex = this.Positions.IndexOf(last);
                        var breakIndex = this.Positions.IndexOf(amBreak);
                        if (currentIndex + 1 == breakIndex)
                        {
                            this.Positions.Remove(amBreak);
                            this.Positions.Insert(breakIndex - 1, amBreak);
                        }
                    }

                    // 2.如果存在下午将下午更改显示名称
                    var amCount = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);
                    var pms = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM)?.ToList();
                    var pmsCount = pms.Count();
                    if (pmsCount > 0)
                    {
                        // 上午节次默认4节
                        for (int i = 1; i <= pmsCount; i++)
                        {
                            pms[i - 1].PeriodString = _periods[amCount + i];
                        }
                    }
                }

            }
            else if (position.PositionType == XYKernel.OS.Common.Enums.Position.PM)
            {
                var pmBreak = this.Positions.FirstOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PB);
                // 没有大课间
                if (pmBreak == null)
                {
                    this.Positions.Remove(position);

                    var count = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);
                    if (count == 0)
                    {
                        this.PmChecked = false;
                        return;
                    }

                    var last = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);
                    last.CanOperation = true;
                }
                else
                {
                    // 常规删除
                    this.Positions.Remove(position);

                    var last = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);
                    last.CanOperation = true;

                    // 删除上午大课间
                    var count = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);
                    if (count == 1)
                    {
                        this.PmBreakChecked = false;
                        this.Positions.Remove(pmBreak);
                        return;
                    }

                    // 验证大课间是否挨着节次
                    var currentIndex = this.Positions.IndexOf(last);
                    var breakIndex = this.Positions.IndexOf(pmBreak);
                    if (currentIndex + 1 == breakIndex)
                    {
                        this.Positions.Remove(pmBreak);
                        this.Positions.Insert(breakIndex - 1, pmBreak);
                        return;
                    }
                }
            }
            else if (position.PositionType == XYKernel.OS.Common.Enums.Position.ES)
            {
                this.Positions.Remove(position);

                var count = this.Positions.Count(p => p.PositionType == XYKernel.OS.Common.Enums.Position.ES);
                if (count == 0)
                {
                    this.EveningStudyChecked = false;
                }
                else
                {
                    var last = this.Positions.LastOrDefault(p => p.PositionType == XYKernel.OS.Common.Enums.Position.ES);
                    last.CanOperation = true;
                }
            }
        }

        void downTime(object obj)
        {
            UIPosition position = obj as UIPosition;

            if (position.PositionType == XYKernel.OS.Common.Enums.Position.AB)
            {
                var amabPositions = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM || p.PositionType == XYKernel.OS.Common.Enums.Position.AB)?.ToList();

                var taskCount = amabPositions.IndexOf(position);

                var positionIndex = this.Positions.IndexOf(position);
                var amPositions = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.AM);

                var filter = amPositions.Take(taskCount);

                var last = amPositions.Except(filter);
                if (last.Count(f => f.PositionType == XYKernel.OS.Common.Enums.Position.AM) > 1)
                {
                    var downAM = this.Positions[positionIndex + 1];

                    this.Positions.Insert(positionIndex + 1, position);
                    this.Positions.Remove(position);

                    this.Positions.Remove(downAM);
                    this.Positions.Insert(positionIndex, downAM);
                }
                else
                {
                    this.ShowDialog("提示信息", "大课间必须设置在节次之间", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                }

            }
            else if (position.PositionType == XYKernel.OS.Common.Enums.Position.PB)
            {
                var pmPositions = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM || p.PositionType == XYKernel.OS.Common.Enums.Position.PB)?.ToList();

                var positionIndex = this.Positions.IndexOf(position);

                var taskCount = pmPositions.IndexOf(position);
                var amPositions = this.Positions.Where(p => p.PositionType == XYKernel.OS.Common.Enums.Position.PM);
                var filter = amPositions.Take(taskCount);

                var last = amPositions.Except(filter);
                if (last.Count(f => f.PositionType == XYKernel.OS.Common.Enums.Position.PM) > 1)
                {
                    var downPM = this.Positions[positionIndex + 1];

                    this.Positions.Insert(positionIndex + 1, position);
                    this.Positions.Remove(position);

                    this.Positions.Remove(downPM);
                    this.Positions.Insert(positionIndex, downPM);
                }
                else
                {
                    this.ShowDialog("提示信息", "大课间必须设置在节次之间", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                }
            }

        }

        void previous(object obj)
        {
            UIPosition position = obj as UIPosition;

            if (position.PositionType == XYKernel.OS.Common.Enums.Position.AB)
            {
                var positionIndex = this.Positions.IndexOf(position);

                var filter = this.Positions.Take(positionIndex);
                if (filter.Count(f => f.PositionType == XYKernel.OS.Common.Enums.Position.AM) > 1)
                {
                    var previousAM = this.Positions[positionIndex - 1];

                    this.Positions.Insert(positionIndex - 1, position);
                    this.Positions.Remove(position);

                    this.Positions.Remove(previousAM);
                    this.Positions.Insert(positionIndex, previousAM);
                }
                else
                {
                    this.ShowDialog("提示信息", "大课间必须设置在节次之间", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                }

            }
            else if (position.PositionType == XYKernel.OS.Common.Enums.Position.PB)
            {
                var positionIndex = this.Positions.IndexOf(position);

                var filter = this.Positions.Take(positionIndex + 1);
                if (filter.Count(f => f.PositionType == XYKernel.OS.Common.Enums.Position.PM) > 1)
                {
                    var previousPM = this.Positions[positionIndex - 1];

                    this.Positions.Insert(positionIndex - 1, position);
                    this.Positions.Remove(position);

                    this.Positions.Remove(previousPM);
                    this.Positions.Insert(positionIndex, previousPM);
                }
                else
                {
                    this.ShowDialog("提示信息", "大课间必须设置在节次之间", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                }
            }

        }

        public void Refresh()
        {
            this.Initilize();
        }

        public void SetColumnPosition(DayOfWeek dayofweek)
        {
            var first = this.Positions.FirstOrDefault();

            switch (dayofweek)
            {
                case DayOfWeek.Monday:

                    var monday = first.IsMondayChecked;

                    foreach (var p in this.Positions)
                    {
                        if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                        {
                            p.IsMondayChecked = !monday;
                        }
                    }

                    break;
                case DayOfWeek.Tuesday:

                    var tuesday = first.IsTuesdayChecked;

                    foreach (var p in this.Positions)
                    {
                        if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                        {
                            p.IsTuesdayChecked = !tuesday;
                        }
                    }

                    break;
                case DayOfWeek.Wednesday:

                    var wednesday = first.IsWednesdayChecked;

                    foreach (var p in this.Positions)
                    {
                        if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                        {
                            p.IsWednesdayChecked = !wednesday;
                        }
                    }

                    break;
                case DayOfWeek.Thursday:

                    var thursday = first.IsThursdayChecked;

                    foreach (var p in this.Positions)
                    {
                        if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                        {
                            p.IsThursdayChecked = !thursday;
                        }
                    }

                    break;
                case DayOfWeek.Friday:

                    var friday = first.IsFridayChecked;

                    foreach (var p in this.Positions)
                    {
                        if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                        {
                            p.IsFridayChecked = !friday;
                        }
                    }

                    break;
                case DayOfWeek.Saturday:

                    var saturday = first.IsSaturdayChecked;

                    foreach (var p in this.Positions)
                    {
                        if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                        {
                            p.IsSaturdayChecked = !saturday;
                        }
                    }

                    break;
                case DayOfWeek.Sunday:

                    var sunday = first.IsSundayChecked;

                    foreach (var p in this.Positions)
                    {
                        if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                            && p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                        {
                            p.IsSundayChecked = !sunday;
                        }
                    }

                    break;
            }
        }
    }
}
