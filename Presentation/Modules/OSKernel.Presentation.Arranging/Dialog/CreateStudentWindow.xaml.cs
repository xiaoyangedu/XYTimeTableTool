using OSKernel.Presentation.Utilities;
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

namespace OSKernel.Presentation.Arranging.Dialog
{
    /// <summary>
    /// 创建学生
    /// </summary>
    public partial class CreateStudentWindow
    {
        /// <summary>
        /// 教师
        /// </summary>
        public List<string> Students { get; set; }

        public CreateStudentWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.txt_student.Focus();
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_student.Text))
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = "学生为空";
            }

            this.Students = txt_student.Text.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None)?.ToList();
            this.Students.RemoveAll(c => string.IsNullOrEmpty(c.Trim()));

            var hasRepeat = this.Students.IsRepeatHashSet();
            if (hasRepeat.Count>0)
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = $"重复学生{hasRepeat.Parse()}";
                return;
            }

            this.DialogResult = true;
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
