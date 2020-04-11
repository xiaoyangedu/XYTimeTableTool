using OSKernel.Presentation.CustomControl;
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
    /// 创建课程
    /// </summary>
    public partial class CreateCourseWindow
    {
        /// <summary>
        /// 科目
        /// </summary>
        public List<string> Courses { get; set; }

        public CreateCourseWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.txt_course.Focus();
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_course.Text))
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = "课程为空";
                return;
            }

            // 清除科目
            this.Courses = txt_course.Text.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None)?.ToList();
            Courses.RemoveAll(c => string.IsNullOrEmpty(c.Trim()));

            var hasRepeat = this.Courses.IsRepeatHashSet();
            if (hasRepeat.Count>0)
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = $"重复科目{hasRepeat.Parse()}";
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
