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

namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    /// <summary>
    /// 创建标签
    /// </summary>
    public partial class CreateTagWindow
    {
        /// <summary>
        /// 教师
        /// </summary>
        public List<string> Tags { get; set; }

        public CreateTagWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.txt_tag.Focus();
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_tag.Text))
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = "标签为空";
                return;
            }

            this.Tags = txt_tag.Text.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None)?.ToList();
            this.Tags.RemoveAll(c => string.IsNullOrEmpty(c.Trim()));

            var hasRepeat = this.Tags.IsRepeatHashSet();
            if (hasRepeat.Count > 0)
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = $"重复标签{hasRepeat.Parse()}";
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
