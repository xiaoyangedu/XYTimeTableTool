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
    /// 创建班级
    /// </summary>
    public partial class CreateClassWindow
    {
        /// <summary>
        /// 班级
        /// </summary>
        public List<string> Classes { get; set; }

        public CreateClassWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.txt_class.Focus();
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_class.Text))
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = "班级为空";
                return;
            }

            this.Classes = txt_class.Text.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None)?.ToList();
            this.Classes.RemoveAll(c => string.IsNullOrEmpty(c.Trim()));

            var hasRepeat = this.Classes.IsRepeatHashSet();
            if (hasRepeat.Count > 0)
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = $"重复班级{hasRepeat.Parse()}";
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
