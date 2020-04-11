using OSKernel.Presentation.Models.Result;
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
    /// ResultWeek
    /// </summary>
    public partial class ResultWeek : UserControl
    {
        public List<UIResultWeek> Periods
        {
            get { return (List<UIResultWeek>)GetValue(PeriodsProperty); }
            set { SetValue(PeriodsProperty, value); }
        }

        public static readonly DependencyProperty PeriodsProperty =
            DependencyProperty.Register("Periods", typeof(List<UIResultWeek>), typeof(ResultWeek), new PropertyMetadata(new PropertyChangedCallback(PeriodsChangedCallback)));

        static void PeriodsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResultWeek control = d as ResultWeek;

            var weeks = e.NewValue as List<UIResultWeek>;
            control.itemControl_period.ItemsSource = weeks;
        }

        public ResultWeek()
        {
            InitializeComponent();
        }
    }
}
