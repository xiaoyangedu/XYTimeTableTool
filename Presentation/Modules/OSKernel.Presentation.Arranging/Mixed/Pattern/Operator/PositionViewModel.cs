using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern.Operator
{
    public class PositionViewModel : CommonViewModel, IInitilize
    {
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

        public PositionViewModel()
        {
            this.Periods = new List<UITwoStatusWeek>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            CLCase clModel = CommonDataManager.GetCLCase(base.LocalID);

            var results = new List<UITwoStatusWeek>();
            var groups = clModel.Positions.GroupBy(p => p.DayPeriod.Period);
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

        /// <summary>
        /// 获取新的课位
        /// </summary>
        /// <returns>课位信息</returns>
        public List<CoursePositionModel> GetCoursePositions()
        {
            List<CoursePositionModel> positions = new List<CoursePositionModel>();
            this.Periods.ForEach(p =>
            {
                positions.Add(new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Monday,
                        Period = p.Period.Period,
                        PeriodName = p.Period.PeriodName
                    },
                    IsSelected = p.Monday.IsChecked,
                    Position = p.PositionType,
                    PositionOrder = p.Period.Period
                });

                positions.Add(new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Tuesday,
                        Period = p.Period.Period,
                        PeriodName = p.Period.PeriodName
                    },
                    IsSelected = p.Tuesday.IsChecked,
                    Position = p.PositionType,
                    PositionOrder = p.Period.Period
                });

                positions.Add(new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Wednesday,
                        Period = p.Period.Period,
                        PeriodName = p.Period.PeriodName
                    },
                    IsSelected = p.Wednesday.IsChecked,
                    Position = p.PositionType,
                    PositionOrder = p.Period.Period
                });

                positions.Add(new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Thursday,
                        Period = p.Period.Period,
                        PeriodName = p.Period.PeriodName
                    },
                    IsSelected = p.Thursday.IsChecked,
                    Position = p.PositionType,
                    PositionOrder = p.Period.Period
                });

                positions.Add(new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Friday,
                        Period = p.Period.Period,
                        PeriodName = p.Period.PeriodName
                    },
                    IsSelected = p.Friday.IsChecked,
                    Position = p.PositionType,
                    PositionOrder = p.Period.Period
                });

                positions.Add(new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Saturday,
                        Period = p.Period.Period,
                        PeriodName = p.Period.PeriodName
                    },
                    IsSelected = p.Saturday.IsChecked,
                    Position = p.PositionType,
                    PositionOrder = p.Period.Period
                });

                positions.Add(new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Sunday,
                        Period = p.Period.Period,
                        PeriodName = p.Period.PeriodName
                    },
                    IsSelected = p.Sunday.IsChecked,
                    Position = p.PositionType,
                    PositionOrder = p.Period.Period
                });
            });

            return positions;
        }
    }
}
