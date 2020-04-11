using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models.Base;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views
{
    /// <summary>
    /// Interaction logic for TimeView.xaml
    /// </summary>
    public partial class TimeView : UserControl
    {
        public TimeViewModel VM
        {
            get
            {
                return this.DataContext as TimeViewModel;
            }
        }

        public TimeView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<TimeViewModel>();
        }

        private void Border_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            VM.SetColumnPosition(DayOfWeek.Monday);
        }

        private void Border_MouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            VM.SetColumnPosition(DayOfWeek.Tuesday);
        }

        private void Border_MouseLeftButtonDown3(object sender, MouseButtonEventArgs e)
        {
            VM.SetColumnPosition(DayOfWeek.Wednesday);
        }

        private void Border_MouseLeftButtonDown4(object sender, MouseButtonEventArgs e)
        {
            VM.SetColumnPosition(DayOfWeek.Thursday);
        }

        private void Border_MouseLeftButtonDown5(object sender, MouseButtonEventArgs e)
        {
            VM.SetColumnPosition(DayOfWeek.Friday);
        }

        private void Border_MouseLeftButtonDown6(object sender, MouseButtonEventArgs e)
        {
            VM.SetColumnPosition(DayOfWeek.Saturday);
        }

        private void Border_MouseLeftButtonDown7(object sender, MouseButtonEventArgs e)
        {
            VM.SetColumnPosition(DayOfWeek.Sunday);
        }

        private void Border_norml_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            UIPosition week = border.DataContext as UIPosition;

            if (week.PositionType != XYKernel.OS.Common.Enums.Position.AB
                && week.PositionType != XYKernel.OS.Common.Enums.Position.PB
                && week.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
            {
                var isChecked = week.IsMondayChecked;

                week.IsMondayChecked = !isChecked;
                week.IsTuesdayChecked = !isChecked;
                week.IsWednesdayChecked = !isChecked;
                week.IsThursdayChecked = !isChecked;
                week.IsFridayChecked = !isChecked;
                week.IsSaturdayChecked = !isChecked;
                week.IsSundayChecked = !isChecked;
            }
        }
    }
}
