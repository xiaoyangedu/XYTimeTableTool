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
    /// 调整结果周课表
    /// </summary>
    public partial class AdjustResultWeek : UserControl
    {
        public List<UIAdjustResultWeek> Periods
        {
            get { return (List<UIAdjustResultWeek>)GetValue(PeriodsProperty); }
            set { SetValue(PeriodsProperty, value); }
        }

        public static readonly DependencyProperty PeriodsProperty =
            DependencyProperty.Register("Periods", typeof(List<UIAdjustResultWeek>), typeof(AdjustResultWeek), new PropertyMetadata(new PropertyChangedCallback(PeriodsChangedCallback)));

        static void PeriodsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdjustResultWeek control = d as AdjustResultWeek;

            var weeks = e.NewValue as List<UIAdjustResultWeek>;
            control.itemControl_period.ItemsSource = weeks;
        }

        public AdjustResultWeek()
        {
            InitializeComponent();
        }
    }
}
