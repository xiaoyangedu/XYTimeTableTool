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
    /// Interaction logic for EmptyView.xaml
    /// </summary>
    public partial class EmptyView : UserControl
    {
        public EmptyView()
        {
            InitializeComponent();
        }

        public string ShowMessage
        {
            get { return (string)GetValue(ShowMessageProperty); }
            set { SetValue(ShowMessageProperty, value); }
        }

        public Visibility ShowMessageVisibility
        {
            get { return (Visibility)GetValue(ShowMessageVisibilityProperty); }
            set { SetValue(ShowMessageVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ShowMessageProperty =
            DependencyProperty.Register("ShowMessage", typeof(string), typeof(EmptyView), new PropertyMetadata("无数据", new PropertyChangedCallback(ShowMessageCallback)));

        public static readonly DependencyProperty ShowMessageVisibilityProperty =
            DependencyProperty.Register("ShowMessageVisibility", typeof(Visibility), typeof(EmptyView), new PropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(ShowMessageVisibilityCallback)));

        public static void ShowMessageCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EmptyView emptyView = d as EmptyView;
            emptyView.txt_NoData.Text = e.NewValue.ToString();
        }

        public static void ShowMessageVisibilityCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EmptyView emptyView = d as EmptyView;
            emptyView.txt_NoData.Visibility = (Visibility)e.NewValue;
        }
    }
}
