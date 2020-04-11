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

namespace OSKernel.Presentation.Arranging.Administrative.Result
{
    /// <summary>
    /// 导出类型窗口
    /// </summary>
    public partial class ExportTypeWindow
    {
        /// <summary>
        /// 类型
        /// </summary>
        public int Type = 1;

        public ExportTypeWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        public ExportTypeWindow(bool showStudent) : this()
        {
            if (showStudent)
            {
                radio_student.Visibility = Visibility.Visible;
            }
            else
            {
                radio_student.Visibility = Visibility.Collapsed;
            }
        }

        private void Radio_class_Click(object sender, RoutedEventArgs e)
        {
            Type = 1;
        }

        private void Radio_teacher_Click(object sender, RoutedEventArgs e)
        {
            Type = 2;
        }

        private void Radio_student_Click(object sender, RoutedEventArgs e)
        {
            Type = 3;
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
