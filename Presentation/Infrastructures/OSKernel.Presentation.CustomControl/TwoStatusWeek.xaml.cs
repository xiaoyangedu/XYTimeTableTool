using OSKernel.Presentation.Models.Base;
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
    /// Interaction logic for TwoStatusTime.xaml
    /// </summary>
    public partial class TwoStatusWeek : UserControl
    {
        public List<UITwoStatusWeek> Periods
        {
            get { return (List<UITwoStatusWeek>)GetValue(PeriodsProperty); }
            set { SetValue(PeriodsProperty, value); }
        }

        public static readonly DependencyProperty PeriodsProperty =
            DependencyProperty.Register("Periods", typeof(List<UITwoStatusWeek>), typeof(TwoStatusWeek), new PropertyMetadata(new PropertyChangedCallback(PeriodsChangedCallback)));

        static void PeriodsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TwoStatusWeek control = d as TwoStatusWeek;

            var weeks = e.NewValue as List<UITwoStatusWeek>;
            control.itemControl_period.ItemsSource = weeks;
        }

        public TwoStatusWeek()
        {
            InitializeComponent();
        }

        private void CheckBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            var week = checkBox.DataContext as UIWeek;

            if (!week.IsChecked)
            {
                week.IsChecked = true;
                week.IsMouseLeft = true;
            }
            else
            {
                week.IsMouseLeft = false;
                week.IsChecked = false;
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

        private void Border_norml_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            UITwoStatusWeek dataContext = border.DataContext as UITwoStatusWeek;
            if (dataContext.PositionType != XYKernel.OS.Common.Enums.Position.AB
                && dataContext.PositionType != XYKernel.OS.Common.Enums.Position.PB
                && dataContext.PositionType != XYKernel.OS.Common.Enums.Position.Noon)
            {
                var isChecked = dataContext.Monday.IsChecked;
                var isLeft = dataContext.Monday.IsMouseLeft;

                if (!isChecked)
                {
                    dataContext.Monday.IsChecked = true;
                    dataContext.Monday.IsMouseLeft = false;

                    dataContext.Tuesday.IsChecked = true;
                    dataContext.Tuesday.IsMouseLeft = false;

                    dataContext.Wednesday.IsChecked = true;
                    dataContext.Wednesday.IsMouseLeft = false;

                    dataContext.Thursday.IsChecked = true;
                    dataContext.Thursday.IsMouseLeft = false;

                    dataContext.Friday.IsChecked = true;
                    dataContext.Friday.IsMouseLeft = false;

                    dataContext.Saturday.IsChecked = true;
                    dataContext.Saturday.IsMouseLeft = false;

                    dataContext.Sunday.IsChecked = true;
                    dataContext.Sunday.IsMouseLeft = false;
                }
                else
                {
                    dataContext.Monday.IsChecked = !isChecked;
                    dataContext.Monday.IsMouseLeft = !isLeft;

                    dataContext.Tuesday.IsChecked = !isChecked;
                    dataContext.Tuesday.IsMouseLeft = !isLeft;

                    dataContext.Wednesday.IsChecked = !isChecked;
                    dataContext.Wednesday.IsMouseLeft = !isLeft;

                    dataContext.Thursday.IsChecked = !isChecked;
                    dataContext.Thursday.IsMouseLeft = !isLeft;

                    dataContext.Friday.IsChecked = !isChecked;
                    dataContext.Friday.IsMouseLeft = !isLeft;

                    dataContext.Saturday.IsChecked = !isChecked;
                    dataContext.Saturday.IsMouseLeft = !isLeft;

                    dataContext.Sunday.IsChecked = !isChecked;
                    dataContext.Sunday.IsMouseLeft = !isLeft;
                }


            }
        }
    }
}
