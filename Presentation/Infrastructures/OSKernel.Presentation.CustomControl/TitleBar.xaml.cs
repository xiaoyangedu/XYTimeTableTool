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
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        public TitleBar()
        {
            InitializeComponent();
        }

        public bool CanChecked
        {
            get { return (bool)GetValue(CanCheckedProperty); }
            set { SetValue(CanCheckedProperty, value); }
        }

        public static readonly DependencyProperty CanCheckedProperty =
            DependencyProperty.Register("CanChecked", typeof(bool), typeof(TitleBar), new PropertyMetadata(false, new PropertyChangedCallback(CanCheckedChangedCallback)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TitleBar), new PropertyMetadata("", new PropertyChangedCallback(TitleChangedCallback)));

        public static void TitleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TitleBar bar = d as TitleBar;
            bar.txt_Title.Text = e.NewValue?.ToString();
            bar.check_box.Content = e.NewValue?.ToString();
        }

        public static void CanCheckedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TitleBar bar = d as TitleBar;
            if ((bool)e.NewValue)
            {
                bar.check_box.Visibility = Visibility.Visible;
                bar.txt_Title.Visibility = Visibility.Collapsed;
            }
            else
            {
                bar.check_box.Visibility = Visibility.Collapsed;
                bar.txt_Title.Visibility = Visibility.Visible;
            }
        }
    }
}
