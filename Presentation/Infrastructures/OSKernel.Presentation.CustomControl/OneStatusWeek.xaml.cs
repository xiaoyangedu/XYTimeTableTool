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
    public partial class OneStatusWeek : UserControl
    {
        public List<UITwoStatusWeek> Periods
        {
            get { return (List<UITwoStatusWeek>)GetValue(PeriodsProperty); }
            set { SetValue(PeriodsProperty, value); }
        }

        public static readonly DependencyProperty PeriodsProperty =
            DependencyProperty.Register("Periods", typeof(List<UITwoStatusWeek>), typeof(OneStatusWeek), new PropertyMetadata(new PropertyChangedCallback(PeriodsChangedCallback)));

        static void PeriodsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OneStatusWeek control = d as OneStatusWeek;

            var weeks = e.NewValue as List<UITwoStatusWeek>;
            control.itemControl_period.ItemsSource = weeks;
        }

        public OneStatusWeek()
        {
            InitializeComponent();
        }

        private void Border_norml_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            UITwoStatusWeek week = border.DataContext as UITwoStatusWeek;

            if (week.PositionType != XYKernel.OS.Common.Enums.Position.AB
                && week.PositionType != XYKernel.OS.Common.Enums.Position.PB
                && week.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
            {
                var isChecked = week.Monday.IsChecked;

                week.Monday.IsChecked = !isChecked;
                week.Tuesday.IsChecked = !isChecked;
                week.Wednesday.IsChecked = !isChecked;
                week.Thursday.IsChecked = !isChecked;
                week.Friday.IsChecked = !isChecked;
                week.Saturday.IsChecked = !isChecked;
                week.Sunday.IsChecked = !isChecked;
            }
        }

        private void Border_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            var first = Periods.FirstOrDefault();
            if (first != null)
            {
                var isChecked = first.Monday.IsChecked;
                var isLeft = first.Monday.IsMouseLeft;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        if (!isChecked)
                        {
                            p.Monday.IsChecked = true;
                            p.Monday.IsMouseLeft = false;
                        }
                        else
                        {
                            p.Monday.IsChecked = !isChecked;
                            p.Monday.IsMouseLeft = !isLeft;
                        }

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
                var isLeft = first.Tuesday.IsMouseLeft;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        if (!isChecked)
                        {
                            p.Tuesday.IsChecked = true;
                            p.Tuesday.IsMouseLeft = false;
                        }
                        else
                        {
                            p.Tuesday.IsChecked = !isChecked;
                            p.Tuesday.IsMouseLeft = !isLeft;
                        }
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
                var isLeft = first.Wednesday.IsMouseLeft;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        if (!isChecked)
                        {
                            p.Wednesday.IsChecked = true;
                            p.Wednesday.IsMouseLeft = false;
                        }
                        else
                        {
                            p.Wednesday.IsChecked = !isChecked;
                            p.Wednesday.IsMouseLeft = !isLeft;
                        }
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
                var isLeft = first.Thursday.IsMouseLeft;

                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        if (!isChecked)
                        {
                            p.Thursday.IsChecked = true;
                            p.Thursday.IsMouseLeft = false;
                        }
                        else
                        {
                            p.Thursday.IsChecked = !isChecked;
                            p.Thursday.IsMouseLeft = !isLeft;
                        }
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
                var isLeft = first.Friday.IsMouseLeft;

                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        if (!isChecked)
                        {
                            p.Friday.IsChecked = true;
                            p.Friday.IsMouseLeft = false;
                        }
                        else
                        {
                            p.Friday.IsChecked = !isChecked;
                            p.Friday.IsMouseLeft = !isLeft;
                        }
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
                var isLeft = first.Saturday.IsMouseLeft;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    || p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        if (!isChecked)
                        {
                            p.Saturday.IsChecked = true;
                            p.Saturday.IsMouseLeft = false;
                        }
                        else
                        {
                            p.Saturday.IsChecked = !isChecked;
                            p.Saturday.IsMouseLeft = !isLeft;
                        }
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
                var isLeft = first.Sunday.IsMouseLeft;
                Periods.ForEach(p =>
                {
                    if (p.PositionType != XYKernel.OS.Common.Enums.Position.AB
                    && p.PositionType != XYKernel.OS.Common.Enums.Position.PB
                    && p.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        if (!isChecked)
                        {
                            p.Sunday.IsChecked = true;
                            p.Sunday.IsMouseLeft = false;
                        }
                        else
                        {
                            p.Sunday.IsChecked = !isChecked;
                            p.Sunday.IsMouseLeft = !isLeft;
                        }
                    }
                });
            }
        }
    }
}
