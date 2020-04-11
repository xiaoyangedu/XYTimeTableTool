using OSKernel.Presentation.Models.Time;
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

namespace OSKernel.Presentation.CustomControl
{
    /// <summary>
    /// Week.xaml 的交互逻辑
    /// </summary>
    public partial class Week : UserControl
    {
        public List<UITwoStatusWeek> Periods
        {
            get { return (List<UITwoStatusWeek>)GetValue(PeriodsProperty); }
            set { SetValue(PeriodsProperty, value); }
        }

        public static readonly DependencyProperty PeriodsProperty =
            DependencyProperty.Register("Periods", typeof(List<UITwoStatusWeek>), typeof(Week), new PropertyMetadata(new PropertyChangedCallback(PeriodsChangedCallback)));

        static void PeriodsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Week control = d as Week;

            var weeks = e.NewValue as List<UITwoStatusWeek>;
            control.itemControl_period.ItemsSource = weeks;
        }

        public Week()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            var first = Periods.FirstOrDefault();
            if (first != null)
            {
                var isChecked = first.Monday.IsChecked;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        p.Monday.IsChecked = !isChecked;
                    }
                });
            }
        }

        private void Border_MouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            var first = Periods.FirstOrDefault();
            if (first != null)
            {
                var isChecked = first.Tuesday.IsChecked;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        p.Tuesday.IsChecked = !isChecked;
                    }
                });
            }
        }

        private void Border_MouseLeftButtonDown3(object sender, MouseButtonEventArgs e)
        {
            var first = Periods.FirstOrDefault();
            if (first != null)
            {
                var isChecked = first.Wednesday.IsChecked;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        p.Wednesday.IsChecked = !isChecked;
                    }
                });
            }
        }

        private void Border_MouseLeftButtonDown4(object sender, MouseButtonEventArgs e)
        {
            var first = Periods.FirstOrDefault();
            if (first != null)
            {
                var isChecked = first.Thursday.IsChecked;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        p.Thursday.IsChecked = !isChecked;
                    }
                });
            }
        }

        private void Border_MouseLeftButtonDown5(object sender, MouseButtonEventArgs e)
        {
            var first = Periods.FirstOrDefault();
            if (first != null)
            {
                var isChecked = first.Friday.IsChecked;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        p.Friday.IsChecked = !isChecked;
                    }
                });
            }
        }

        private void Border_MouseLeftButtonDown6(object sender, MouseButtonEventArgs e)
        {
            var first = Periods.FirstOrDefault();
            if (first != null)
            {
                var isChecked = first.Saturday.IsChecked;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        p.Saturday.IsChecked = !isChecked;
                    }
                });
            }
        }

        private void Border_MouseLeftButtonDown7(object sender, MouseButtonEventArgs e)
        {
            var first = Periods.FirstOrDefault();
            if (first != null)
            {
                var isChecked = first.Sunday.IsChecked;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    && p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    && p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        p.Sunday.IsChecked = !isChecked;
                    }
                });
            }
        }

        private void Border_norml_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            UITwoStatusWeek dataContext = border.DataContext as UITwoStatusWeek;
            if (dataContext.PositionType != XYKernel.OS.Common.Enums.Position.AB
                && dataContext.PositionType != XYKernel.OS.Common.Enums.Position.PB
                && dataContext.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
            {
                var isChecked = !dataContext.Monday.IsChecked;

                dataContext.Monday.IsChecked = isChecked;
                dataContext.Tuesday.IsChecked = isChecked;
                dataContext.Wednesday.IsChecked = isChecked;
                dataContext.Thursday.IsChecked = isChecked;
                dataContext.Friday.IsChecked = isChecked;
                dataContext.Saturday.IsChecked = isChecked;
                dataContext.Sunday.IsChecked = isChecked;
            }
        }
    }
}
