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
    /// 创建教师
    /// </summary>
    public partial class CreateTeacherWindow
    {
        /// <summary>
        /// 教师
        /// </summary>
        public List<string> Teachers { get; set; }

        public CreateTeacherWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.txt_teacher.Focus();
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_teacher.Text))
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = "教师为空";
                return;
            }

            this.Teachers = txt_teacher.Text.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None)?.ToList();
            this.Teachers.RemoveAll(c => string.IsNullOrEmpty(c.Trim()));

            var hasRepeat = this.Teachers.IsRepeatHashSet();
            if (hasRepeat.Count > 0)
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = $"重复教师{hasRepeat.Parse()}";
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
