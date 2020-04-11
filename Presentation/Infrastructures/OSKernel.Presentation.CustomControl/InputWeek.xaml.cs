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
    public partial class InputWeek : UserControl
    {
        public List<UITwoStatusWeek> Periods
        {
            get { return (List<UITwoStatusWeek>)GetValue(PeriodsProperty); }
            set { SetValue(PeriodsProperty, value); }
        }

        public static readonly DependencyProperty PeriodsProperty =
            DependencyProperty.Register("Periods", typeof(List<UITwoStatusWeek>), typeof(InputWeek), new PropertyMetadata(new PropertyChangedCallback(PeriodsChangedCallback)));

        static void PeriodsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InputWeek control = d as InputWeek;

            var weeks = e.NewValue as List<UITwoStatusWeek>;
            control.itemControl_period.ItemsSource = weeks;
        }

        public InputWeek()
        {
            InitializeComponent();
        }
    }
}
