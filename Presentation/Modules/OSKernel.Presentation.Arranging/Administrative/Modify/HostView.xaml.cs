using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour;
using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.Teacher;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.ClassHour;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OSKernel.Presentation.Arranging.Administrative.Modify
{
    /// <summary>
    /// 用来承载教师高级规则窗口
    /// </summary>
    public partial class HostView
    {
        public HostView()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.Unloaded += HostView_Unloaded;
        }

        public HostView(AdministrativeAlgoRuleEnum ruleEnum) : this()
        {
            this.Title = $"高级-{ruleEnum.GetLocalDescription()}";

            if (ruleEnum == AdministrativeAlgoRuleEnum.ClassHourRequiredStartingTime)
            {
                FillContainer(new RequiredStartingTimeView());
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.ClassHourRequiredStartingTimes ||
                ruleEnum == AdministrativeAlgoRuleEnum.ClassHourRequiredTimes)
            {
                FillContainer(new ClassHourRequiredTimes(ruleEnum));
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.ClassHoursNotOverlap)
            {
                FillContainer(new HoursNotOverlapView());
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime ||
                ruleEnum == AdministrativeAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection)
            {
                FillContainer(new ClassHoursTimesValue(ruleEnum));
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.ClassHoursRequiredStartingTimes ||
                ruleEnum == AdministrativeAlgoRuleEnum.ClassHoursRequiredTimes)
            {
                FillContainer(new ClassHoursRequiredTimes(ruleEnum));
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.ClassHoursSameStarting ||
                ruleEnum == AdministrativeAlgoRuleEnum.ClassHoursSameStartingHour ||
                ruleEnum == AdministrativeAlgoRuleEnum.ClassHoursSameStartingTime)
            {
                FillContainer(new HoursSameStartingView(ruleEnum));
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.MaxDaysBetweenClassHours ||
                ruleEnum == AdministrativeAlgoRuleEnum.MinDaysBetweenClassHours)
            {
                FillContainer(new BetweenClassHoursView(ruleEnum));
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.ThreeClassHoursGrouped
                || ruleEnum == AdministrativeAlgoRuleEnum.TwoClassHoursContinuous
                || ruleEnum == AdministrativeAlgoRuleEnum.TwoClassHoursGrouped
                || ruleEnum == AdministrativeAlgoRuleEnum.TwoClassHoursOrdered)
            {
                FillContainer(new MultipyClassHoursView(ruleEnum));
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.TeacherMaxDaysPerWeek
                 || ruleEnum == AdministrativeAlgoRuleEnum.TeachersMaxDaysPerWeek)
            {
                FillContainer(new Algo.Teacher.MaxDaysPerWeekView(ruleEnum));
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.TeacherMaxGapsPerDay
                || ruleEnum == AdministrativeAlgoRuleEnum.TeachersMaxGapsPerDay)
            {
                FillContainer(new Algo.Teacher.MaxGapsPerDayView(ruleEnum));
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.TeacherMaxHoursDaily
               || ruleEnum == AdministrativeAlgoRuleEnum.TeachersMaxHoursDaily)
            {

                FillContainer(new Algo.Teacher.MaxHoursDailyView(ruleEnum));

            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.TeacherNotAvailableTimes)
            {
                FillContainer(new NotAvailableTimesView());
            }
        }

        public HostView(AdministrativeRuleEnum ruleEnum) : this()
        {
            this.Title = $"规则-{ruleEnum.GetLocalDescription()}";

            if (ruleEnum == AdministrativeRuleEnum.AmPmClassHour)
            {
                FillContainer(new AmPmClassHourView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.ArrangeContinuousPlanFlush)
            {
                FillContainer(new ArrangeContinuousPlanFlush());
            }
            else if (ruleEnum == AdministrativeRuleEnum.ClassHourAverage)
            {
                FillContainer(new ClassHourAverageView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.ClassHourSameOpen)
            {
                FillContainer(new ClassHourSameOpenView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.ClassUnion)
            {
                FillContainer(new ClassUnionView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.CourseArrangeContinuous)
            {
                FillContainer(new CourseArrangeContinuous());
            }
            else if (ruleEnum == AdministrativeRuleEnum.CourseLimit)
            {
                FillContainer(new CourseLimitView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.CourseTime)
            {
                FillContainer(new CourseTimeView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.MasterApprenttice)
            {
                FillContainer(new MasterApprentticeView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.MutexGroup)
            {
                FillContainer(new MutexGroupView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.OddDualWeek)
            {
                FillContainer(new OddDualWeekView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.TeacherAmPmNoContinues)
            {
                FillContainer(new AmPmNoContinuesView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.TeacherMaxDaysPerWeek)
            {
                FillContainer(new Rule.Teacher.MaxDaysPerWeekView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.TeacherMaxGapsPerDay)
            {
                FillContainer(new Rule.Teacher.MaxGapsPerDayView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.TeacherMaxHoursDaily)
            {
                FillContainer(new Rule.Teacher.MaxHoursDailyView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.TeacherTime)
            {
                FillContainer(new TeacherTimeView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.TeachingPlanFlush)
            {
                FillContainer(new PlanFlushView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.TeacherPriorityBalanceRule)
            {
                FillContainer(new TeacherPriorityBalanceView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.TeacherHalfDayWorkRule)
            {
                FillContainer(new TeacherHalfDayWorkView());
            }
            else if (ruleEnum == AdministrativeRuleEnum.LockedCourse)
            {
                FillContainer(new LockedCourseView());
            }
        }

        void FillContainer(UIElement element)
        {
            container.Content = element;
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(this);
        }

        private void HostView_Unloaded(object sender, RoutedEventArgs e)
        {
            IDisposable disposable = this.container.Content as IDisposable;
            disposable?.Dispose();
        }
    }
}
