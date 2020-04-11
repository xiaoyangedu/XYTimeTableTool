using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour;
using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.ClassHour;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Room;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher;
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

namespace OSKernel.Presentation.Arranging.Mixed.Modify
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

        public HostView(MixedAlgoRuleEnum ruleEnum) : this()
        {
            this.Title = $"高级-{ruleEnum.GetLocalDescription()}";

            if (ruleEnum == MixedAlgoRuleEnum.ClassHourRequiredStartingTime)
            {
                FillContainer(new RequiredStartingTimeView());
            }
            else if (ruleEnum == MixedAlgoRuleEnum.ClassHourRequiredStartingTimes ||
                ruleEnum == MixedAlgoRuleEnum.ClassHourRequiredTimes)
            {
                FillContainer(new ClassHourRequiredTimes(ruleEnum));
            }
            else if (ruleEnum == MixedAlgoRuleEnum.ClassHoursNotOverlap)
            {
                FillContainer(new HoursNotOverlapView());
            }
            else if (ruleEnum == MixedAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime ||
                ruleEnum == MixedAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection)
            {
                FillContainer(new ClassHoursTimesValue(ruleEnum));
            }
            else if (ruleEnum == MixedAlgoRuleEnum.ClassHoursRequiredStartingTimes ||
                ruleEnum == MixedAlgoRuleEnum.ClassHoursRequiredTimes)
            {
                FillContainer(new ClassHoursRequiredTimes(ruleEnum));
            }
            else if (ruleEnum == MixedAlgoRuleEnum.ClassHoursSameStarting ||
                ruleEnum == MixedAlgoRuleEnum.ClassHoursSameStartingHour ||
                ruleEnum == MixedAlgoRuleEnum.ClassHoursSameStartingTime)
            {
                FillContainer(new HoursSameStartingView(ruleEnum));
            }
            else if (ruleEnum == MixedAlgoRuleEnum.MaxDaysBetweenClassHours ||
                ruleEnum == MixedAlgoRuleEnum.MinDaysBetweenClassHours)
            {
                FillContainer(new BetweenClassHoursView(ruleEnum));
            }
            else if (ruleEnum == MixedAlgoRuleEnum.ThreeClassHoursGrouped
                || ruleEnum == MixedAlgoRuleEnum.TwoClassHoursContinuous
                || ruleEnum == MixedAlgoRuleEnum.TwoClassHoursGrouped
                || ruleEnum == MixedAlgoRuleEnum.TwoClassHoursOrdered)
            {
                FillContainer(new MultipyClassHoursView(ruleEnum));
            }
            else if (ruleEnum == MixedAlgoRuleEnum.TeacherMaxDaysPerWeek
                 || ruleEnum == MixedAlgoRuleEnum.TeachersMaxDaysPerWeek)
            {
                FillContainer(new Algo.Teacher.MaxDaysPerWeekView(ruleEnum));
            }
            else if (ruleEnum == MixedAlgoRuleEnum.TeacherMaxGapsPerDay
                || ruleEnum == MixedAlgoRuleEnum.TeachersMaxGapsPerDay)
            {
                FillContainer(new Algo.Teacher.MaxGapsPerDayView(ruleEnum));
            }
            else if (ruleEnum == MixedAlgoRuleEnum.TeacherMaxHoursDaily
               || ruleEnum == MixedAlgoRuleEnum.TeachersMaxHoursDaily)
            {
                FillContainer(new Algo.Teacher.MaxHoursDailyView(ruleEnum));
            }
            else if (ruleEnum == MixedAlgoRuleEnum.TeacherNotAvailableTimes)
            {
                FillContainer(new NotAvailableTimesView());
            }
        }

        public HostView(MixedRuleEnum ruleEnum) : this()
        {
            this.Title = $"规则-{ruleEnum.GetLocalDescription()}";

            if (ruleEnum == MixedRuleEnum.AmPmClassHour)
            {
                FillContainer(new AmPmClassHourView());
            }
            else if (ruleEnum == MixedRuleEnum.ClassHourAverage)
            {
                FillContainer(new ClassHourAverageView());
            }
            else if (ruleEnum == MixedRuleEnum.ClassHourSameOpen)
            {
                FillContainer(new ClassHourSameOpenView());
            }
            else if (ruleEnum == MixedRuleEnum.CourseArrangeContinuous)
            {
                FillContainer(new CourseArrangeContinuous());
            }
            else if (ruleEnum == MixedRuleEnum.CourseLimit)
            {
                FillContainer(new CourseLimitView());
            }
            else if (ruleEnum == MixedRuleEnum.CourseTime)
            {
                FillContainer(new CourseTimeView());
            }
            //else if (ruleEnum == MixedRuleEnum.MasterApprenttice)
            //{
            //    FillContainer(new MasterApprentticeView());
            //}
            else if (ruleEnum == MixedRuleEnum.TeacherMaxDaysPerWeek)
            {
                FillContainer(new Rule.Teacher.MaxDaysPerWeekView());
            }
            else if (ruleEnum == MixedRuleEnum.TeacherMaxGapsPerDay)
            {
                FillContainer(new Rule.Teacher.MaxGapsPerDayView());
            }
            else if (ruleEnum == MixedRuleEnum.TeacherMaxHoursDaily)
            {
                FillContainer(new Rule.Teacher.MaxHoursDailyView());
            }
            else if (ruleEnum == MixedRuleEnum.TeacherTime)
            {
                FillContainer(new TeacherTimeView());
            }
            else if (ruleEnum == MixedRuleEnum.AvailableRoom)
            {
                FillContainer(new AvaliableRoomView());
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
